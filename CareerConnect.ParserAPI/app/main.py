from fastapi import FastAPI
from app.models import CVRequest, ParsedCV
from app.parser import parse_cv

app = FastAPI()

@app.post("/parse-cv", response_model=ParsedCV)
def parse(cv: CVRequest):
    return parse_cv(cv.text)