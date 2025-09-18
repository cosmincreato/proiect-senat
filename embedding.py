import os
import json
from sentence_transformers import SentenceTransformer

input_dir = 'proiectSenat/chunked_output'
sentence_entries = []

for filename in os.listdir(input_dir):
    filepath = os.path.join(input_dir, filename)
    main, chunk_part = filename.split('_')
    year = main[:2]
    if year.startswith('9'):
        year = '19' + year
    else:
        year = '20' + year
    year = int(year)
    law = main[2:]
    chunk_str = chunk_part.replace('chunk', '')
    chunk_num = int(os.path.splitext(chunk_str)[0])
    if os.path.isfile(filepath):
        with open(filepath, encoding='utf-8') as f:
            for line in f:
                line = line.strip()
                if line:
                    sentence_entries.append({
                        "text": line,
                        "year": year,
                        "law": law,
                        "chunk_num": chunk_num,
                        "filename": filename
                    })

sentences = [entry['text'] for entry in sentence_entries]

model = SentenceTransformer('sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2')

print("Starting text embedding...")
embeddings = model.encode(sentences, show_progress_bar=True)

data = []
for idx, (entry, emb) in enumerate(zip(sentence_entries, embeddings)):
    data.append({
        "id": idx,
        "vector": emb.tolist(),
        "payload": {
            "text": entry["text"],
            "an": entry["year"],
            "numar_lege": entry["law"],
            "chunk": entry["chunk_num"],
            "fisier": entry["filename"]
        }
    })

with open('proiectSenat/embeddings.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2)