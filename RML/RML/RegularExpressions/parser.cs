//
// assembly:	System
// namespace:	System.Text.RegularExpressions
// file:	parser.cs
//
// author:	Dan Lewis (dlewis@gmx.co.uk)
// 		(c) 2002

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Iib.RegexMarkupLanguage.RegularExpressions.Syntax {

	class Parser {
		public static int ParseDecimal (string str, ref int ptr) {
			return ParseNumber (str, ref ptr, 10, 1, Int32.MaxValue);
		}

		public static int ParseOctal (string str, ref int ptr) {
			return ParseNumber (str, ref ptr, 8, 1, 3);
		}

		public static int ParseHex (string str, ref int ptr, int digits) {
			return ParseNumber (str, ref ptr, 16, digits, digits);
		}

		public static int ParseNumber (string str, ref int ptr, int b, int min, int max) {
			int p = ptr, n = 0, digits = 0, d;
			if (max < min)
				max = Int32.MaxValue;

			while (digits < max && p < str.Length) {
				d = ParseDigit (str[p ++], b, digits);
				if (d < 0) {
					-- p;
					break;
				}

				n = n * b + d;
				++ digits;
			}

			if (digits < min)
				return -1;

			ptr = p;
			return n;
		}

		public static string ParseName (string str, ref int ptr) {
			if (Char.IsDigit (str[ptr])) {
				int gid = ParseNumber (str, ref ptr, 10, 1, 0);
				if (gid > 0)
					return gid.ToString ();
				
				return null;
			}

			int start = ptr;
			for (;;) {
				if (!IsNameChar (str[ptr]))
					break;
				++ ptr;
			}

			if (ptr - start > 0)
				return str.Substring (start, ptr - start);

			return null;
		}

		public static string Escape (string str) {
			string result = "";
			for (int i = 0; i < str.Length; ++ i) {
				char c = str[i];
				switch (c) {
				case '\\': case '*': case '+': case '?': case '|':
				case '{': case '[': case '(': case ')': case '^':
				case '$': case '.': case '#': case ' ':
					result += "\\" + c;
					break;

				case '\t': result += "\\t"; break;
				case '\n': result += "\\n"; break;
				case '\r': result += "\\r"; break;
				case '\f': result += "\\f"; break;

				default: result += c; break;
				}
			}

			return result;
		}

		public static string Unescape (string str) {
			if (str.IndexOf ('\\') == -1)
				return str;
			return new Parser ().ParseString (str);
		}

		// public instance

		public Parser () {
			this.caps = new ArrayList ();
			this.refs = new Hashtable ();
		}

		public RegularExpression ParseRegularExpression (string pattern, RegexOptions options) {
			this.pattern = pattern;
			this.ptr = 0;

			caps.Clear ();
			refs.Clear ();
			this.num_groups = 0;

			try {
				RegularExpression re = new RegularExpression ();
				ParseGroup (re, options, null);
				ResolveReferences ();

				re.GroupCount = num_groups;
				
				return re;
			}
			catch (IndexOutOfRangeException) {
				throw NewParseException ("Unexpected end of pattern.");
			}
		}

		public IDictionary GetMapping () {
			Hashtable mapping = new Hashtable ();
			int end = caps.Count;
			mapping.Add ("0", 0);
			for (int i = 0; i < end; i++) {
				CapturingGroup group = (CapturingGroup) caps [i];
				if (group.Name != null) {
					if (mapping.Contains (group.Name)) {
						if ((int) mapping [group.Name] != group.Number)
							throw new SystemException ("invalid state");
						continue;
					}
					mapping.Add (group.Name, group.Number);
				} else {
					mapping.Add (group.Number.ToString (), group.Number);
				}
			}

			return mapping;
		}

		// private methods

		private void ParseGroup (Group group, RegexOptions options, Assertion assertion) {
			bool is_top_level = group is RegularExpression;
		
			Alternation alternation = null;
			Interleave interleave = null;
			string literal = null;

			Group current = new Group ();
			Expression expr = null;
			bool closed = false;

			while (true) {
				ConsumeWhitespace (IsIgnorePatternWhitespace (options));
				if (ptr >= pattern.Length)
					break;
				
				// (1) Parse for Expressions
			
				char ch = pattern[ptr ++];
				
				switch (ch) {
				case '^': {
					Position pos =
						IsMultiline (options) ? Position.StartOfLine : Position.Start;
					expr = new PositionAssertion (pos);
					break;
				}

				case '$': {
					Position pos =
						IsMultiline (options) ? Position.EndOfLine : Position.End;
					expr = new PositionAssertion (pos);
					break;
				}

				case '.': {
					Category cat =
						IsSingleline (options) ? Category.AnySingleline : Category.Any;
					expr = new CharacterClass (cat, false);
					break;
				}

				case '\\': {
					int c = ParseEscape ();
					if (c >= 0)
						ch = (char)c;
					else {
						expr = ParseSpecial (options);

						if (expr == null)
							ch = pattern[ptr ++];		// default escape
					}
					break;
				}

				case '[': {
					expr = ParseCharacterClass (options);
					break;
				}

				case '(': {
					bool ignore = IsIgnoreCase (options);
					expr = ParseGroupingConstruct (ref options);
					if (expr == null) {
						if (literal != null && IsIgnoreCase (options) != ignore) {
							current.AppendExpression (new Literal (literal, IsIgnoreCase (options)));
							literal = null;
						}

						continue;
					}
					break;
				}

				case ')': {
					closed = true;
					goto EndOfGroup;
				}
        
        /****************************************************************************************
         * Angepasst wegen Interleave-Befehl.
         * 
         * Date: 30.03.2007
         * Author: Marco Vergari (marco@vergari.ch)
         ****************************************************************************************/
				case '|': {
					if (literal != null) {
						current.AppendExpression (new Literal (literal, IsIgnoreCase (options)));
						literal = null;
					}

					if (assertion != null) {
						if (assertion.TrueExpression == null)
							assertion.TrueExpression = current;
						else if (assertion.FalseExpression == null)
							assertion.FalseExpression = current;
						else
							throw NewParseException ("Too many | in (?()|).");
					}
					else {
						if (alternation == null) {
							alternation = new Alternation ();
					  }

            if(interleave != null) {
              interleave.AddInterleave(current);
              alternation.AddAlternative(interleave);
              interleave = null;
            } else {
						  alternation.AddAlternative(current);
						}
					}

					current = new Group ();
					continue;
				}
				
				/****************************************************************************************
         * Interleave-Befehl abbilden. Ist gleich aufgebaut wie der Alternation-Befehl.
         * 
         * Beispiel: (regex1 & regex2 & regex3)
         * 
         * Date: 27.03.2007
         * Author: Marco Vergari (marco@vergari.ch)
         ****************************************************************************************/
				case '&': {
          if(literal != null) {
					  current.AppendExpression(new Literal(literal, IsIgnoreCase (options)));
					  literal = null;
          }

          if(interleave == null) {
            interleave = new Interleave();
          }
					interleave.AddInterleave(current);
					current = new Group ();
					continue;
				}				

				case '*': case '+': case '?': {
					throw NewParseException ("Bad quantifier.");
				}

				default: 
					break;		// literal character
				}

				ConsumeWhitespace (IsIgnorePatternWhitespace (options));
				
				// (2) Check for Repetitions
				
				if (ptr < pattern.Length) {
					char k = pattern[ptr];
					int min = 0, max = 0;
					bool lazy = false;
					bool haveRep = false;


					if (k == '?' || k == '*' || k == '+') {
						++ ptr;
						haveRep = true;

						switch (k) {
						case '?': min = 0; max = 1; break;
						case '*': min = 0; max = 0x7fffffff; break;
						case '+': min = 1; max = 0x7fffffff; break;
						}
					} else if (k == '{' && ptr + 1 < pattern.Length) {
						int saved_ptr = ptr;
						++ptr;
						haveRep = ParseRepetitionBounds (out min, out max, options);
						if (!haveRep)
							ptr = saved_ptr;
					}

					if (haveRep) {
						ConsumeWhitespace (IsIgnorePatternWhitespace (options));
						if (ptr < pattern.Length && pattern[ptr] == '?') {
							++ ptr;
							lazy = true;
						}

						Repetition repetition = new Repetition (min, max, lazy);

						if (expr == null)
							repetition.Expression = new Literal (ch.ToString (), IsIgnoreCase (options));
						else
							repetition.Expression = expr;

						expr = repetition;
					}
				}

				// (3) Append Expression and/or Literal

				if (expr == null) {
					if (literal == null)
						literal = "";
					literal += ch;
				}
				else {
					if (literal != null) {
						current.AppendExpression (new Literal (literal, IsIgnoreCase (options)));
						literal = null;
					}

					current.AppendExpression (expr);
					expr = null;
				}

				if (is_top_level && ptr >= pattern.Length)
					goto EndOfGroup;
			}

		EndOfGroup:
			if (is_top_level && closed)
				throw NewParseException ("Too many )'s.");
			if (!is_top_level && !closed)
				throw NewParseException ("Not enough )'s.");
				
		
			// clean up literals and alternations

			if (literal != null)
				current.AppendExpression (new Literal (literal, IsIgnoreCase (options)));

			if (assertion != null) {
				if (assertion.TrueExpression == null)
					assertion.TrueExpression = current;
				else
					assertion.FalseExpression = current;
				
				group.AppendExpression (assertion);
			}
			else if (alternation != null) {  /* Angepasst wegen Interleave Befehl. Author: Marco Vergari (marco@vergari.ch) */
			  if(interleave != null) {
		      interleave.AddInterleave(current);
		      alternation.AddAlternative(interleave);
			  } else {
  				alternation.AddAlternative(current);
			  }
			  group.AppendExpression(alternation);
			}
			else if(interleave != null) {  /* Author: Marco Vergari (marco@vergari.ch) */
			  interleave.AddInterleave(current);
			  group.AppendExpression(interleave);
			} else {
				group.AppendExpression(current);
			}
		}

		private Expression ParseGroupingConstruct (ref RegexOptions options) {
			if (pattern[ptr] != '?') {
				Group group;

				if (IsExplicitCapture (options))
					group = new Group ();
				else {
					group = new CapturingGroup ();
					caps.Add (group);
				}

				ParseGroup (group, options, null);
				return group;
			}
			else
				++ ptr;

			switch (pattern[ptr]) {
			case ':': {						// non-capturing group
				++ ptr;
				Group group = new Group ();
				ParseGroup (group, options, null);

				return group;
			}

			case '>': {						// non-backtracking group
				++ ptr;
				Group group = new NonBacktrackingGroup ();
				ParseGroup (group, options, null);
				
				return group;
			}

			case 'i': case 'm': case 'n':
			case 's': case 'x': case '-': {				// options
				RegexOptions o = options;
				ParseOptions (ref o, false);
				if (pattern[ptr] == '-') {
					++ ptr;
					ParseOptions (ref o, true);
				}

				if (pattern[ptr] == ':') {			// pass options to child group
					++ ptr;
					Group group = new Group ();
					ParseGroup (group, o, null);
					return group;
				}
				else if (pattern[ptr] == ')') {			// change options of enclosing group
					++ ptr;
					options = o;
					return null;
				}
				else
					throw NewParseException ("Bad options");
			}

			case '<': case '=': case '!': {				// lookahead/lookbehind
				ExpressionAssertion asn = new ExpressionAssertion ();
				if (!ParseAssertionType (asn))
					goto case '\'';				// it's a (?<name> ) construct

				Group test = new Group ();
				ParseGroup (test, options, null);

				asn.TestExpression = test;
				return asn;
			}

			case '\'': {						// named/balancing group
				char delim;
				if (pattern[ptr] == '<')
					delim = '>';
				else
					delim = '\'';

				++ ptr;
				string name = ParseName ();

				if (pattern[ptr] == delim) {
					// capturing group

					if (name == null)
						throw NewParseException ("Bad group name.");

					++ ptr;
					CapturingGroup cap = new CapturingGroup ();
					cap.Name = name;
					caps.Add (cap);
					ParseGroup (cap, options, null);

					return cap;
				}
				else if (pattern[ptr] == '-') {
					// balancing group

					++ ptr;
					string balance_name = ParseName ();
					if (balance_name == null || pattern[ptr] != delim)
						throw NewParseException ("Bad balancing group name.");

					++ ptr;
					BalancingGroup bal = new BalancingGroup ();
					bal.Name = name;
					
					if(bal.IsNamed) {
						caps.Add (bal);
					}

					refs.Add (bal, balance_name);

					ParseGroup (bal, options, null);

					return bal;
				}
				else
					throw NewParseException ("Bad group name.");
			}

			case '(': {						// expression/capture test
				Assertion asn;
			
				++ ptr;
				int p = ptr;
				string name = ParseName ();
				if (name == null || pattern[ptr] != ')') {	// expression test
					// FIXME MS implementation doesn't seem to
					// implement this version of (?(x) ...)

					ptr = p;
					ExpressionAssertion expr_asn = new ExpressionAssertion ();

					if (pattern[ptr] == '?') {
						++ ptr;
						if (!ParseAssertionType (expr_asn))
							throw NewParseException ("Bad conditional.");
					}
					else {
						expr_asn.Negate = false;
						expr_asn.Reverse = false;
					}

					Group test = new Group ();
					ParseGroup (test, options, null);
					expr_asn.TestExpression = test;
					asn = expr_asn;
				}
				else {						// capture test
					++ ptr;
					asn = new CaptureAssertion (new Literal (name, IsIgnoreCase (options)));
					refs.Add (asn, name);
				}

				Group group = new Group ();
				ParseGroup (group, options, asn);
				return group;
			}

			case '#': {						// comment
				++ ptr;
				while (pattern[ptr ++] != ')') {
					if (ptr >= pattern.Length)
						throw NewParseException ("Unterminated (?#...) comment.");
				}
				return null;
			}

			default: 						// error
				throw NewParseException ("Bad grouping construct.");
			}
		}

		private bool ParseAssertionType (ExpressionAssertion assertion) {
			if (pattern[ptr] == '<') {
				switch (pattern[ptr + 1]) {
				case '=':
					assertion.Negate = false;
					break;
				case '!':
					assertion.Negate = true;
					break;
				default:
					return false;
				}

				assertion.Reverse = true;
				ptr += 2;
			}
			else {
				switch (pattern[ptr]) {
				case '=':
					assertion.Negate = false;
					break;
				case '!':
					assertion.Negate = true;
					break;
				default:
					return false;
				}

				assertion.Reverse = false;
				ptr += 1;
			}

			return true;
		}

		private void ParseOptions (ref RegexOptions options, bool negate) {
			for (;;) {
				switch (pattern[ptr]) {
				case 'i':
					if (negate)
						options &= ~RegexOptions.IgnoreCase;
					else
						options |= RegexOptions.IgnoreCase;
					break;

				case 'm':
					if (negate)
						options &= ~RegexOptions.Multiline;
					else
						options |= RegexOptions.Multiline;
					break;
					
				case 'n':
					if (negate)
						options &= ~RegexOptions.ExplicitCapture;
					else
						options |= RegexOptions.ExplicitCapture;
					break;
					
				case 's':
					if (negate)
						options &= ~RegexOptions.Singleline;
					else
						options |= RegexOptions.Singleline;
					break;
					
				case 'x':
					if (negate)
						options &= ~RegexOptions.IgnorePatternWhitespace;
					else
						options |= RegexOptions.IgnorePatternWhitespace;
					break;

				default:
					return;
				}

				++ ptr;
			}
		}

		private Expression ParseCharacterClass (RegexOptions options) {
			bool negate = false;
			if (pattern[ptr] == '^') {
				negate = true;
				++ ptr;
			}
			
			bool ecma = IsECMAScript (options);
			CharacterClass cls = new CharacterClass (negate, IsIgnoreCase (options));

			if (pattern[ptr] == ']') {
				cls.AddCharacter (']');
				++ ptr;
			}

			int c = -1;
			int last = -1;
			bool range = false;
			bool closed = false;
			while (ptr < pattern.Length) {
				c = pattern[ptr ++];

				if (c == ']') {
					closed = true;
					break;
				}

				if (c == '-' && last >= 0 && !range) {
					range = true;
					continue;
				}

				if (c == '\\') {
					c = ParseEscape ();
					if (c >= 0)
						goto char_recognized;

					// didn't recognize escape
					c = pattern [ptr ++];
					switch (c) {
					case 'b':
						c = '\b';
						goto char_recognized;

					case 'd': case 'D':
						cls.AddCategory (ecma ? Category.EcmaDigit : Category.Digit, c == 'D');
						break;
						
					case 'w': case 'W':
						cls.AddCategory (ecma ? Category.EcmaWord : Category.Word, c == 'W');
						break;
						
					case 's': case 'S':
						cls.AddCategory (ecma ? Category.EcmaWhiteSpace : Category.WhiteSpace, c == 'S');
						break;
						
					case 'p': case 'P':
						cls.AddCategory (ParseUnicodeCategory (), c == 'P');	// ignore ecma
						break;

					default:		// add escaped character
						goto char_recognized;
					}

					// if the pattern looks like [a-\s] ...
					if (range)
						throw NewParseException ("character range cannot have category \\" + c);

					last = -1;
					continue;
				}

			char_recognized:
				if (range) {
					// if 'range' is true, we know that 'last >= 0'
					if (c < last)
						throw NewParseException ("[" + last + "-" + c + "] range in reverse order.");
					cls.AddRange ((char)last, (char)c);
					last = -1;
					range = false;
					continue;
				}

				cls.AddCharacter ((char)c);
				last = c;
			}

			if (!closed)
				throw NewParseException ("Unterminated [] set.");

			if (range)
				cls.AddCharacter ('-');

			return cls;
		}

		private bool ParseRepetitionBounds (out int min, out int max, RegexOptions options) {
			int n, m;
			min = max = 0;

			/* check syntax */

			ConsumeWhitespace (IsIgnorePatternWhitespace (options));
		    
			if (pattern[ptr] == ',') {
                                n = -1;
			} else {
                                n = ParseNumber (10, 1, 0);
                                ConsumeWhitespace (IsIgnorePatternWhitespace (options));
			}
			
			switch (pattern[ptr ++]) {
			case '}':
				m = n;
				break;
			case ',':
				ConsumeWhitespace (IsIgnorePatternWhitespace (options));
				m = ParseNumber (10, 1, 0);
				ConsumeWhitespace (IsIgnorePatternWhitespace (options));
				if (pattern[ptr ++] != '}')
					return false;
				break;
			default:
				return false;
			}

			/* check bounds and ordering */

			if (n >= 0xffff || m >= 0xffff)
				throw NewParseException ("Illegal {x, y} - maximum of 65535.");
			if (m >= 0 && m < n)
				throw NewParseException ("Illegal {x, y} with x > y.");

			/* assign min and max */
			
			min = n;
			if (m > 0)
				max = m;
			else
				max = 0xffff;

			return true;
		}

		private Category ParseUnicodeCategory () {
			if (pattern[ptr ++] != '{')
				throw NewParseException ("Incomplete \\p{X} character escape.");

			string name = ParseName (pattern, ref ptr);
			if (name == null)
				throw NewParseException ("Incomplete \\p{X} character escape.");

			Category cat = CategoryUtils.CategoryFromName (name);
			if (cat == Category.None)
				throw NewParseException ("Unknown property '" + name + "'.");

			if (pattern[ptr ++] != '}')
				throw NewParseException ("Incomplete \\p{X} character escape.");

			return cat;
		}
		
		/****************************************************************************************
     * List den Namen des externen Aufrufs ein. Bedingung ist das nach einem externen Aufruf
     * eine schliessende Klammer ')' kommt. Der externe Aufruf muss immer am Ende einer Gruppe
     * stehen.
     * 
     * Beispiel: (test-regex\C{Class.Method})
     * 
     * Date: 20.12.2006
     * Author: Marco Vergari (marco@vergari.ch)
     ****************************************************************************************/
		private string ParseExtCallName() {
			if(pattern[ptr++] != '{') {
				throw NewParseException ("Incomplete \\C{Name} external call escape.");
		  }

			string name = null;
		  int start = ptr;
			for(;;) {
				if(!(Char.IsLetterOrDigit(pattern[ptr]) || (pattern[ptr] == '.'))) {
					break;
				}
				++ptr;
			}

			if((ptr - start) > 0) {
				name = pattern.Substring(start, ptr - start);
			} else {
				throw NewParseException ("Incomplete \\C{Name} external call escape.");
		  }

			if(pattern[ptr++] != '}') {
			  throw NewParseException ("Incomplete \\C{Name} external call escape.");
			}
			if(pattern[ptr] != ')') {
			  throw NewParseException ("External call \\C{Name} must end with closing group character.");
			}
  		return name;
		}

		private Expression ParseSpecial (RegexOptions options) {
			int p = ptr;
			bool ecma = IsECMAScript (options);
			Expression expr = null;
			
			switch (pattern[ptr ++]) {

			// categories

			case 'd':
				expr = new CharacterClass (ecma ? Category.EcmaDigit : Category.Digit, false);
				break;
				
			case 'w':
				expr = new CharacterClass (ecma ? Category.EcmaWord : Category.Word, false);
				break;
				
			case 's':
				expr = new CharacterClass (ecma ? Category.EcmaWhiteSpace : Category.WhiteSpace, false);
				break;
				
			case 'p':
				// this is odd - ECMAScript isn't supposed to support Unicode,
				// yet \p{..} compiles and runs under the MS implementation
				// identically to canonical mode. That's why I'm ignoring the
				// value of ecma here.
			
				expr = new CharacterClass (ParseUnicodeCategory (), false);
				break;
				
			/***************************************************************************************
       * Wenn ein \C im Regex vorhanden ist handelt sichs um einen externen Aufruf. Dies ist
       * jedoch nur erlaubt wenn die Regexoption "ExplicitCapture" nicht gesetzt ist. Dies ist
       * so weil an dieser Stelle nicht erkannt werden kann ob eine schliessende Klammer zu
       * einer CaptureGroup oder zu einer gew�hnlichen Group geh�rt. Der Befehl \C bezieht
       * sich auf Daten und dies ist nur m�glich wenn es sich um eine CaptureGroup handelt.
       * 
       * Beispiel: (test-regex\C{Class.Method})
       * 
       * Date: 20.12.2006
       * Author: Marco Vergari (marco@vergari.ch)
       ****************************************************************************************/
			case 'C':
			  if(IsExplicitCapture(options)) {
			    throw NewParseException ("Regexoption 'ExplicitCapture' is not allowed when external call \\C{Name} is used.");
			  }
	      expr = new ExtCall(ParseExtCallName(), false);
  		break;
				
			case 'D':
				expr = new CharacterClass (ecma ? Category.EcmaDigit : Category.Digit, true);
				break;
				
			case 'W':
				expr = new CharacterClass (ecma ? Category.EcmaWord : Category.Word, true);
				break;
				
			case 'S':
				expr = new CharacterClass (ecma ? Category.EcmaWhiteSpace : Category.WhiteSpace, true);
				break;
				
			case 'P':
				expr = new CharacterClass (ParseUnicodeCategory (), true);
				break;

			// positions

			case 'A': expr = new PositionAssertion (Position.StartOfString); break;
			case 'Z': expr = new PositionAssertion (Position.End); break;
			case 'z': expr = new PositionAssertion (Position.EndOfString); break;
			case 'G': expr = new PositionAssertion (Position.StartOfScan); break;
			case 'b': expr = new PositionAssertion (Position.Boundary); break;
			case 'B': expr = new PositionAssertion (Position.NonBoundary); break;
			
			// references

			case '1': case '2': case '3': case '4': case '5':
			case '6': case '7': case '8': case '9': {
				ptr --;
				int n = ParseNumber (10, 1, 0);
				if (n < 0) {
					ptr = p;
					return null;
				}

				// FIXME test if number is within number of assigned groups
				// this may present a problem for right-to-left matching

				Reference reference = new Reference (IsIgnoreCase (options));
				refs.Add (reference, n.ToString ());
				expr = reference;
				break;
			}

			case 'k': {
				char delim = pattern[ptr ++];
				if (delim == '<')
					delim = '>';
				else if (delim != '\'')
					throw NewParseException ("Malformed \\k<...> named backreference.");

				string name = ParseName ();
				if (name == null || pattern[ptr] != delim)
					throw NewParseException ("Malformed \\k<...> named backreference.");

				++ ptr;
				Reference reference = new Reference (IsIgnoreCase (options));
				refs.Add (reference, name);
				expr = reference;
				break;
			}

			default:
				expr = null;
				break;
			}

			if (expr == null)
				ptr = p;

			return expr;
		}

		private int ParseEscape () {
			int p = ptr;
			int c;

			if (p >= pattern.Length)
				throw new ArgumentException (
						String.Format ("Parsing \"{0}\" - Illegal \\ at end of " + 
								"pattern.", pattern), pattern);
			
			switch (pattern[ptr ++]) {
	
			// standard escapes (except \b)

			case 'a': return '\u0007';
			case 't': return '\u0009';
			case 'r': return '\u000d';
			case 'v': return '\u000b';
			case 'f': return '\u000c';
			case 'n': return '\u000a';
			case 'e': return '\u001b';
			case '\\': return '\\';

			// character codes

			case '0':
				//
				// Turns out that octal values can be specified
				// without a leading zero.   But also the limit
				// of three character should include this first
				// one.  
				//
				ptr--;
				int prevptr = ptr;
				int result = ParseOctal (pattern, ref ptr);
				if (result == -1 && prevptr == ptr)
					return 0;

				return result;

			case 'x':
				c = ParseHex (pattern, ref ptr, 2);
				if (c < 0)
					throw NewParseException ("Insufficient hex digits");

				return c;

			case 'u':
				c = ParseHex (pattern, ref ptr, 4);
				if (c < 0)
					throw NewParseException ("Insufficient hex digits");
				
				return c;

			// control characters

			case 'c':
				c = pattern[ptr ++];
				if (c >= '@' && c <= '_')
					return c - '@';
				else
					throw NewParseException ("Unrecognized control character.");

			// unknown escape

			default:
				ptr = p;
				return -1;
			}
		}

		private string ParseName () {
			return Parser.ParseName (pattern, ref ptr);
		}

		private static bool IsNameChar (char c) {
			UnicodeCategory cat = Char.GetUnicodeCategory (c);
			if (cat == UnicodeCategory.ModifierLetter)
				return false;
			if (cat == UnicodeCategory.ConnectorPunctuation)
				return true;
			return Char.IsLetterOrDigit (c);
		}
	
		private int ParseNumber (int b, int min, int max) {
			return Parser.ParseNumber (pattern, ref ptr, b, min, max);
		}

		private static int ParseDigit (char c, int b, int n) {
			switch (b) {
			case 8:
				if (c >= '0' && c <= '7')
					return c - '0';
				else
					return -1;
			case 10:
				if (c >= '0' && c <= '9')
					return c - '0';
				else
					return -1;
			case 16:
				if (c >= '0' && c <= '9')
					return c - '0';
				else if (c >= 'a' && c <= 'f')
					return 10 + c - 'a';
				else if (c >= 'A' && c <= 'F')
					return 10 + c - 'A';
				else
					return -1;
			default:
				return -1;
			}
		}

		private void ConsumeWhitespace (bool ignore) {
			while (ptr < pattern.Length) {
				if (pattern[ptr] == '(') {
					if (ptr + 3 >= pattern.Length)
						return;

					if (pattern[ptr + 1] != '?' || pattern[ptr + 2] != '#')
						return;

					ptr += 3;
					while (ptr < pattern.Length && pattern[ptr ++] != ')')
						/* ignore */ ;
				}
				else if (ignore && pattern[ptr] == '#') {
					while (ptr < pattern.Length && pattern[ptr ++] != '\n')
						/* ignore */ ;
				}
				else if (ignore && Char.IsWhiteSpace (pattern[ptr])) {
					while (ptr < pattern.Length && Char.IsWhiteSpace (pattern[ptr]))
						++ ptr;
				}
				else
					return;
			}
		}

		private string ParseString (string pattern) {
			this.pattern = pattern;
			this.ptr = 0;

			StringBuilder result = new StringBuilder (pattern.Length);
			while (ptr < pattern.Length) {
				int c = pattern[ptr ++];
				if (c == '\\') {
					c = ParseEscape ();

					if(c < 0) {
						c = pattern[ptr ++];
						if(c == 'b')
							c = '\b';
					}
				}
				result.Append ((char) c);
			}

			return result.ToString ();
		}

		private void ResolveReferences () {
			int gid = 1;
			Hashtable dict = new Hashtable ();

			// number unnamed groups

			foreach (CapturingGroup group in caps) {
				if (group.Name == null) {
					dict.Add (gid.ToString (), group);
					group.Number = gid ++;

					++ num_groups;
				}
			}

			// number named groups

			foreach (CapturingGroup group in caps) {
				if (group.Name != null) {
					if (!dict.Contains (group.Name)) {
						dict.Add (group.Name, group);
						group.Number = gid ++;

						++ num_groups;
					}
					else {
						CapturingGroup prev = (CapturingGroup)dict[group.Name];
						group.Number = prev.Number;
					}
				}
			}

			// resolve references

			foreach (Expression expr in refs.Keys) {
				string name = (string)refs[expr];
				if (!dict.Contains (name)) {
				  if (expr is CaptureAssertion && !Char.IsDigit (name [0]))
						continue;
					throw NewParseException ("Reference to undefined group " +
						(Char.IsDigit (name[0]) ? "number " : "name ") +
						name);
				}

				CapturingGroup group = (CapturingGroup)dict[name];
				if (expr is Reference)
					((Reference)expr).CapturingGroup = group;
				else if (expr is CaptureAssertion)
					((CaptureAssertion)expr).CapturingGroup = group;
				else if (expr is BalancingGroup)
					((BalancingGroup)expr).Balance = group;
			}
		}

		// flag helper functions

		private static bool IsIgnoreCase (RegexOptions options) {
			return (options & RegexOptions.IgnoreCase) != 0;
		}

		private static bool IsMultiline (RegexOptions options) {
			return (options & RegexOptions.Multiline) != 0;
		}

		private static bool IsExplicitCapture (RegexOptions options) {
			return (options & RegexOptions.ExplicitCapture) != 0;
		}
	
		private static bool IsSingleline (RegexOptions options) {
			return (options & RegexOptions.Singleline) != 0;
		}

		private static bool IsIgnorePatternWhitespace (RegexOptions options) {
			return (options & RegexOptions.IgnorePatternWhitespace) != 0;
		}

		private static bool IsECMAScript (RegexOptions options) {
			return (options & RegexOptions.ECMAScript) != 0;
		}

		// exception creation

		private ArgumentException NewParseException (string msg) {
			msg = "parsing \"" + pattern + "\" - " + msg;
			return new ArgumentException (msg, pattern);
		}

		private string pattern;
		private int ptr;

		private ArrayList caps;
		private Hashtable refs;
		private int num_groups;
	}
}
