"""
Authors: Ioannis Vettas & Pascal Haupt
"""

from utils import hamming_distance

def _get_callsign(context_result):
    if context_result:
        return context_result['CallSign']
    return None

def check_callsign(context_result, label_data):
    expected_airline = label_data['airline']
    expected_flightnr = label_data['flightnr']
    actual_airline = None
    actual_flightnr = None
    
    callsign = _get_callsign(context_result)
    if callsign:
        actual_airline = callsign['Airline'].strip()
        actual_flightnr = callsign['FlightNumber'].strip()

    return (
        actual_airline == expected_airline,
        actual_flightnr == expected_flightnr,
        hamming_distance(actual_airline, expected_airline) if actual_airline is not None else None,
        hamming_distance(actual_flightnr, expected_flightnr) if actual_flightnr is not None else None
    )

def _get_intents(context_result):
    intents = {}
    if context_result:
        for name,value in context_result['Intents'].items():
            intents[name.lower()] = value
    return intents

def check_intents(context_result, label_data, required_score = 0.5):
    detectedIntents = []
    expected_intents = label_data['intent']

    for intentName, intentData in _get_intents(context_result).items():
        if intentData is not None and intentData['Score'] >= required_score:
            detectedIntents.append(intentName)
    
    missingIntents = list(set(expected_intents) - set(detectedIntents))
    redundantIntents = list(set(detectedIntents) - set(expected_intents))

    return (
        detectedIntents == expected_intents,
        detectedIntents,
        missingIntents,
        redundantIntents
    )
