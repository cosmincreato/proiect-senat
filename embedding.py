import os
from sentence_transformers import SentenceTransformer
import csv

input_dir = 'proiectSenat/chunked_output'
sentences = []
for filename in os.listdir(input_dir):
    filepath = os.path.join(input_dir, filename)
    if os.path.isfile(filepath):
        with open(filepath, encoding='utf-8') as f:
            sentences.extend([line.strip() for line in f if line.strip()])

model = SentenceTransformer('sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2')
embeddings = model.encode(sentences, show_progress_bar=True)

with open('proiectSenat/embeddings.csv', 'w', newline='', encoding='utf-8') as f:
    writer = csv.writer(f)
    writer.writerow(['sentence'] + [f'emb_{i}' for i in range(len(embeddings[0]))])
    for sentence, emb in zip(sentences, embeddings):
        writer.writerow([sentence] + list(emb))