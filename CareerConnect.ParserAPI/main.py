from fastapi import FastAPI
from app.models import CVRequest, ParsedCV
from app.parser import parse_cv
from dotenv import load_dotenv
from openai import OpenAI
import os

load_dotenv()  # .env dosyasını yükler

client = OpenAI(api_key=os.getenv("OPENAI_API_KEY"))
app = FastAPI()

@app.post("/parse-cv", response_model=ParsedCV)
async def parse(cv: CVRequest, client):
    return await parse_cv(cv.text)