# Proiect Senat

This project is developed as part of my internship at the Romanian Senate. It is a modern Retrieval-Augmented Generation pipeline designed to process and embed large legal corpora, enabling semantic search and contextually-aware responses with Large Language Models.

![alt text](https://i.imgur.com/5FPuxzi.png)
---

## Features

- **Text Chunking & Embedding**: Preprocesses and splits large documents, then maps sentences & paragraphs to a 384 dimensional dense vector space using the [`sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2`](https://huggingface.co/sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2) model.
- **Embeddings Storage**: Stores embeddings and associated text segments in `embeddings.json` for downstream retrieval.
- **RAG Workflow**: Enables retrieval of relevant text segments in response to user queries, providing augmented prompts to LLMs for improved answers.
- **Cross-language Orchestration**: Utilizes C# for process orchestration and possible integration into .NET applications, while calling Python scripts for automation.

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/cosmincreato/proiectSenat.git
cd proiectSenat
```

### 2. Python Environment Setup

- **Python version:** 3.8 or newer

```bash
python3 -m venv venv
source venv/bin/activate
```

**Install dependencies:**

```bash
pip install -r requirements.txt
```

---

### 3. Start the Embedding Server

You need to run the embedding server to generate vector representations for your documents.

**Start the server:**

```bash
uvicorn embed_server:app --host 0.0.0.0 --port 8000
```
- The server will be available at [http://localhost:8000](http://localhost:8000)
- Make sure the server stays running while you use the application.

---

### 4. .NET Environment Setup

- **.NET version:** .NET 6.0 or newer
- Restore NuGet packages if prompted.

---

### 5. Qdrant Vector Database Setup

**Recommended:** Use Docker to run Qdrant locally.

#### Start Qdrant Server

```bash
docker run -p 6333:6333 -p 6334:6334 qdrant/qdrant
```

- **REST API:** [http://localhost:6333](http://localhost:6333)
- **gRPC API:** [http://localhost:6334](http://localhost:6334)

**Optional (persistent storage):**
```bash
# On Linux/macOS
docker run -p 6333:6333 -p 6334:6334 \
  -v $(pwd)/qdrant_storage:/qdrant/storage \
  qdrant/qdrant

# On Windows (PowerShell)
docker run -p 6333:6333 -p 6334:6334 `
  -v ${PWD}/qdrant_storage:/qdrant/storage `
  qdrant/qdrant
```

#### Create a Collection

You can create a collection using:
- The browser dashboard
- The REST API:
  ```bash
  curl -X PUT "http://localhost:6333/collections/proiect-senat" \
    -H "Content-Type: application/json" \
    -d '{ "vectors": { "size": 1536, "distance": "Cosine" } }'
  ```
  *(Adjust "size" and "distance" to match your embeddings)*

#### Update Collection Name in Code

In `Program.cs`, update the uploader:

```csharp
var uploader = new QdrantUploader("localhost", 6334, "proiect-senat");
```
Make sure the collection name matches exactly.