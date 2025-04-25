from pydantic import BaseModel
from typing import List

class CVRequest(BaseModel):
    text: str

class ParsedCV(BaseModel):
    skills: List[str]
    education: List[str]
    experience: List[str]
