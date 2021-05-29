"""
Authors: Ioannis Vettas & Pascal Haupt
"""

import sys
from collections import namedtuple
from pathlib import Path, PurePath
from typing import List
from json_utils import load_json_file

__path_data_folder = '../../data/'
__path_label_folder = PurePath(__path_data_folder, 'context-extraction/labelled')
__path_context_folder = PurePath(__path_data_folder, 'context-extraction/extracted')
__path_luisversions_folder = PurePath(__path_data_folder, 'luis-comparison')
__path_rmlversions_folder = PurePath(__path_data_folder, 'rml-comparison')
__path_nbest_folder = PurePath(__path_data_folder, 'speech-to-text/nbest/3best')
__path_cleaned_folder = PurePath('../wer/data/clean/text')


Data = namedtuple('Data', ['filenames', 'labels', 'contexts', 'datarecords'])
DataRecord = namedtuple('DataRecord', ['name', 'label', 'context'])

DataV = namedtuple('DataV', ['versions', 'datarecords'])
DataVRecord = namedtuple('DataVRecord', ['name', 'label', 'contexts'])
NBest = namedtuple('NBest', ['name', 'transcriptions'])


this = sys.modules[__name__]
this.data = None


def __load_labels(folderpath):
    label_data = {}
    for x in Path(folderpath).iterdir():
        if x.is_file() and x.suffix == '.json':
            label_data[x.name] = load_json_file(x)
    return label_data

def __load_contexts(folderpath):
    context_data = {}
    for x in Path(folderpath).iterdir():
        if x.is_file() and x.suffix == '.json':
            context_data[x.name] = load_json_file(x)
    return context_data


def load_rmlluis_data():
    loaded_labels = __load_labels(__path_label_folder)
    loaded_contexts = __load_contexts(__path_context_folder)

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

def load_luisversions_data(luisVersions):
    loaded_labels = __load_labels(__path_label_folder)
    loaded_contexts = {v:__load_contexts(PurePath(__path_luisversions_folder, v)) for v in luisVersions}

    filenames = loaded_labels.keys()
    #labels = []
    #contextslist = []
    datarecords = []

    for filename in filenames:
        name = filename.split('.')[0]
        label = loaded_labels.get(filename)
        contexts = {v:loaded_contexts[v].get(filename) for v in luisVersions}

        #labels.append(label)
        #contextslist.append(contexts)
        datarecords.append(DataVRecord(name, label, contexts))
    
    return DataV(luisVersions, datarecords)

def load_rmlversions_data(rmlVersions):
    loaded_labels = __load_labels(__path_label_folder)
    loaded_contexts = {v:__load_contexts(PurePath(__path_rmlversions_folder, v)) for v in rmlVersions}

    filenames = loaded_labels.keys()
    datarecords = []

    for filename in filenames:
        name = filename.split('.')[0]
        label = loaded_labels.get(filename)
        contexts = {v:loaded_contexts[v].get(filename) for v in rmlVersions}

        datarecords.append(DataVRecord(name, label, contexts))
    
    return DataV(rmlVersions, datarecords)

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
