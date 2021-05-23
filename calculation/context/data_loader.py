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
__path_nbest_folder = PurePath(__path_data_folder, 'stt-nbest')
__path_cleaned_folder = PurePath('../wer/data/clean/text')


Data = namedtuple('Data', ['filenames', 'labels', 'contexts', 'datarecords'])
DataRecord = namedtuple('DataRecord', ['name', 'label', 'context'])

NBest = namedtuple('NBest', ['name', 'transcriptions'])


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


def load_stt_nbests():
    loaded_nbest = {}
    for x in Path(__path_nbest_folder).iterdir():
        if x.is_file() and x.suffix == '.json':
            loaded_nbest[x.name] = load_json_file(x)
    
    nbest_transcriptions = []

    filenames = loaded_nbest.keys()
    for filename in filenames:
        name = filename.split('.')[0]
        trascriptions = loaded_nbest.get(filename)
        nbest_transcriptions.append(NBest(name, trascriptions))
    
    return nbest_transcriptions


def load_human_transcription(filename):
    filecontent = ''
    filepath = Path(__path_cleaned_folder.as_posix() + '\\' + filename + '.txt')
    if(filepath.exists() and filepath.is_file()):
        with open(filepath) as f:
            content = f.readlines()
            filecontent = content[0]

    return filecontent
