# Proiect Senat

This project is developed as part of my internship at the Romanian Senate. It is a modern Retrieval-Augmented Generation pipeline designed to process and embed large legal corpora, enabling semantic search and contextually-aware responses with Large Language Models.

---

## Features

- **Text Chunking & Embedding**: Preprocesses and splits large documents, then generates vector embeddings using the `sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2` model.
- **Embeddings Storage**: Stores embeddings and associated text segments in `embeddings.csv` for downstream retrieval.
- **RAG Workflow**: Enables retrieval of relevant text segments in response to user queries, providing augmented prompts to LLMs for improved answers.
- **Cross-language Orchestration**: Utilizes C# for process orchestration and possible integration into .NET applications.
- **Script Automation**: Seamless calling of Python scripts (`embedding.py`) from C# for automated embedding generation.

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
pip install -U sentence-transformers
pip install numpy<2.0
```

### 3. .NET Environment Setup

- **.NET version:** .NET 6.0 or newer
- Restore NuGet packages if prompted.

### 4. Qdrant Vector Database Setup

- **Recommended:** Use Docker to run Qdrant locally.

```bash
docker run -p 6333:6333 qdrant/qdrant
```

- This command will start Qdrant on [http://localhost:6333](http://localhost:6333)

**To persist data between runs:**
```bash
docker run -p 6333:6333 -v qdrant_data:/qdrant/storage qdrant/qdrant
```