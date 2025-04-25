import requests
import json
import re

async def parse_cv(text: str):
    payload = {
        "model": "phi",
        "prompt": f"""You are an AI assistant. Extract the Skills, Education and Experience from the following CV text. 
Return the output as a JSON object with fields 'skills', 'education', and 'experience'.

CV Text:
{text}
"""
    }
    
    response = requests.post("http://localhost:11434/api/generate", json=payload)
    result = response.json()

    generated_text = result['response']

    print(generated_text, flush=True)

    try:
        extracted = extract_json_from_text(generated_text)
    except json.JSONDecodeError:
        raise Exception(f"Model output is not valid JSON: {generated_text}")

    return extracted

def extract_json_from_text(text: str):
    # İlk { ve son } arasını al
    match = re.search(r'\{.*\}', text, re.DOTALL)
    if match:
        json_str = match.group(0)
        return json.loads(json_str)
    else:
        raise json.JSONDecodeError("No JSON found", text, 0)
