# ProiectSenat

**Work in progress:** This project is developed as part of my internship at the Romanian Senate. It is a .NET 8.0 console application that processes legislative documents (PDF) and provides a chatbot for answering questions about Romanian laws.

## Project Goals

- Extract text from large sets of Romanian legislative PDFs using OCR (Tesseract) and direct extraction.
- Index and analyze these documents for question-answering via a chatbot interface.
- Support Romanian queries about legislative content.

## Main Features

- **PDF Processing:** Direct text extraction and OCR (Tesseract with Romanian language support).
- **Bulk Handling:** Designed for high-volume document processing.
- **Text Indexing:** Automatically indexes processed documents for search and retrieval.
- **Q&A Chatbot:** Command-line interface to ask questions about the processed documents using simple natural language.

## Technologies

- .NET 8.0
- Tesseract OCR
- PdfiumViewer, PdfPig
- Ollama