import sys
from text_cleanup import clean_up_text

# Wrapper for the text2digits python library
# To install text2digits run
# > pip3 install text2digits

if __name__ == '__main__':
    texts = sys.argv[1]
    
    texts = texts.split('||')
    for text in texts:
        print(clean_up_text(text), end='||')