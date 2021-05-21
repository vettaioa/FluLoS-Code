"""
Authors: Ioannis Vettas & Pascal Haupt
"""

flatten = lambda t: [item for sublist in t for item in sublist]

def hamming_distance(string1, string2): 
    length = max(len(string1), len(string2))
    
    string1 = string1.ljust(length)
    string2 = string2.ljust(length)
    
    distance = 0
    for i in range(length):
        if string1[i] != string2[i]:
            distance += 1
    return distance
