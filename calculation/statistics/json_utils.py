"""
Authors: Ioannis Vettas & Pascal Haupt
"""

import json

def load_json_file(filepath):
    with open(filepath) as file:
        return json.load(file)

def prettyfy_json(json_string):
    data = json.loads(json_string)
    return json.dumps(data, indent = 4, sort_keys = True)
