import requests
from openai import OpenAI
import json
import re
import os 

async def parse_cv(text: str, client: OpenAI):

    PROMPT = """You are an AI assistant. Extract the Skills, Education and Experience from the following CV text. 
Return the output strictly as a JSON object with fields 'skills', 'education', and 'experience'.

Here is an example:

CV Text:
John Doe is a software engineer with 5 years of experience. He has worked at Google as a backend developer between 2018 and 2021.
He also worked at Meta from 2021 to 2023. He graduated from MIT with a BSc in Computer Science in 2016.
His skills include Python, Django, and PostgreSQL.

Output:
{{
  "skills": ["Python", "Django", "PostgreSQL"],
  "education": [
    "MIT",
    "BSc",
    "Computer Science"
  ],
  "experience": [
    "Google",
    "Meta",
    "Backend Developer"
  ]
}}

Now, extract the same fields from this CV:

CV Text:
{text}
"""
    try:
        # OpenAI API ile CV'yi analiz et
        response = client.responses.create(
            model="gpt-3.5-turbo",
            input=[
                {"role": "system", "content": "You are a helpful assistant that extracts structured data from resumes."},
                {"role": "user", "content": PROMPT.format(text=text)}
            ],
            temperature=0,
            max_tokens=2000,
        )
        generated_text = response.output_text

        print(generated_text, flush=True)
        json_data = extract_json_from_text(generated_text)
        return json_data
    except Exception as e:
        raise e


def extract_json_from_text(text: str):
    # İlk { ve son } arasını al
    match = re.search(r'\{.*\}', text, re.DOTALL)
    if match:
        json_str = match.group(0)
        return json.loads(json_str)
    else:
        raise json.JSONDecodeError("No JSON found", text, 0)
