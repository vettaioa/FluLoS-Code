"""
Authors: Ioannis Vettas & Pascal Haupt
"""

import sys
from collections import namedtuple
from pathlib import Path, PurePath
from typing import List
from json_utils import load_json_file

__path_data_folder = '../../data/'
__path_label_folder = PurePath(__path_data_folder, 'context-labelled')
__path_context_folder = PurePath(__path_data_folder, 'context-results')

Data = namedtuple('Data', ['filenames', 'labels', 'contexts', 'datarecords'])
DataRecord = namedtuple('DataRecord', ['name', 'label', 'context'])


this = sys.modules[__name__]
this.data = None


def __load_labels():
    label_data = {}
    for x in Path(__path_label_folder).iterdir():
        if x.is_file() and x.suffix == '.json':
            label_data[x.name] = load_json_file(x)
    return label_data

def __load_contexts():
    context_data = {}
    for x in Path(__path_context_folder).iterdir():
        if x.is_file() and x.suffix == '.json':
            context_data[x.name] = load_json_file(x)
    return context_data


def load_data():
    loaded_labels = __load_labels()
    loaded_contexts = __load_contexts()

    filenames = loaded_labels.keys()
    labels = []
    contexts = []
    datarecords = []

    for filename in filenames:
        name = filename.split('.')[0]
        label = loaded_labels.get(filename)
        context = loaded_contexts.get(filename)

        labels.append(label)
        contexts.append(context)
        datarecords.append(DataRecord(name, label, context))
    
    this.data = Data(filenames, labels, contexts, datarecords)
    return this.data

