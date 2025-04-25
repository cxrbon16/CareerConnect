from gliner import GLiNER
import torch


device = "cuda" if torch.cuda.is_available() else "cpu"

model = GLiNER.from_pretrained("urchade/gliner_multi-v2.1", device=device)

def parse_cv(text: str):
    
    print(text)

    skill_entities = ["Skill", "Tools", "Programming Languages", "Ability", "Capability", "Yetenek", "Programlama Dili", "Programming"]
    education_entities = ["Education", "University", "High School", "School", "Institute", "Degree", "Eğitim", "Üniversite", "Lise", "Enstitü"]
    experience_entities = ["Experience", "Company", "Years"]

    labels = skill_entities + education_entities + experience_entities

    predictions = model.predict_entities(text, labels)

    
    skills = [p["text"] for p in predictions if p["label"] in skill_entities]
    education = [p["text"] for p in predictions if p["label"] in education_entities]
    experience = [p["text"] for p in predictions if p["label"] in experience_entities]

    return {
        "skills": skills,
        "education": education,
        "experience": experience
    }
