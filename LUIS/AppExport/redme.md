# LUIS App Exports

These exports can be imported to get the LUIS application used in this work.
This can be achieved in the applications overview at https://www.luis.ai/applications.
After importing, the model must me manually trained and published in LUIS.

## Versions
- **0.1**: Intents are trained with machine learned entities and subentities.
- **0.2-features**: Like 0.1 but intents have machine learned entities as ML-Features.
- **0.2-entities**: Like 0.1 but with additional (separate) list and regex entities to help identify intents.
- **0.3**: Combination of 0.2-features and 0.2-entities.
- **0.4**: The list and regex entities are assigned to machine learned entities as ML-Features.
