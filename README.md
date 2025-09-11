# ProiectSenat

A .NET 8.0 console application for processing and analyzing PDF documents using OCR (Optical Character Recognition) and machine learning technologies.

## 🎯 Project Overview

This project is designed to extract and process text content from large collections of PDF documents, specifically handling Romanian language content using Tesseract OCR with Romanian language support.

## 🔧 Technologies Used

- **.NET 8.0** - Core framework
- **PdfiumViewer** - PDF rendering and image conversion
- **PdfPig** - Alternative PDF text extraction
- **Tesseract OCR** - Optical Character Recognition (Romanian language support)
- **Microsoft.ML** - Machine learning capabilities
- **TorchSharp** - Deep learning framework with CUDA support
- **System.Drawing.Common** - Image processing

## 📁 Project Structure

```
proiectSenat/
├── proiectSenat/
│   ├── PdfOcrProcessor.cs      # Main OCR processing logic
│   ├── proiectSenat.csproj     # Project configuration
│   └── Program.cs              # Application entry point
├── input/                      # PDF files to be processed
├── output/                     # Processed results
├── tessdata/                   # Tesseract language data files
│   └── ron.traineddata        # Romanian language model
└── proiectSenat.sln           # Solution file
```

## 🚀 Features

### PDF Processing
- **Bulk PDF Processing**: Handles large collections of PDF files (tested with 16,000+ documents)
- **Text Extraction**: Attempts direct text extraction first, falls back to OCR for image-based PDFs
- **High-Quality Rendering**: Converts PDF pages to 300 DPI images for optimal OCR accuracy

### OCR Capabilities
- **Romanian Language Support**: Specialized OCR for Romanian text with custom character whitelist
- **Smart Character Recognition**: Includes Romanian diacritics (ĂÂÎȘȚăâîșț)
- **Error Handling**: Robust error handling for corrupted or problematic PDF files

### Text Processing
- **Page-by-Page Extraction**: Maintains document structure with page separators
- **Clean Output**: Filters and processes text for better readability

## 📦 Dependencies

```xml
<PackageReference Include="Microsoft.ML" Version="4.0.2" />
<PackageReference Include="PdfPig" Version="0.1.11" />
<PackageReference Include="PdfiumViewer" Version="2.13.0" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
<PackageReference Include="Tesseract" Version="5.2.0" />
<PackageReference Include="TorchSharp" Version="0.105.1" />
<PackageReference Include="TorchSharp-cuda-windows" Version="0.105.1" />
```

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Windows OS (for CUDA support)
- NVIDIA GPU (optional, for TorchSharp CUDA acceleration)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/cosmincreato/proiectSenat.git
   cd proiectSenat
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Setup Tesseract language data**
   - Create a `tessdata` folder in the project root
   - Download `ron.traineddata` from [Tesseract tessdata repository](https://github.com/tesseract-ocr/tessdata/blob/main/ron.traineddata)
   - Place the file in the `tessdata` folder

4. **Prepare input directory**
   - Create an `input` folder in the project root
   - Place your PDF files in this directory

5. **Build and run**
   ```bash
   dotnet build
   dotnet run
   ```

## 💻 Usage

### Basic Usage

```csharp
var processor = new PdfOcrProcessor();
string extractedText = processor.ExtractTextFromPdf("path/to/document.pdf");
Console.WriteLine(extractedText);
```

### Custom Tessdata Path

```csharp
var processor = new PdfOcrProcessor(@"C:\custom\path\to\tessdata");
string extractedText = processor.ExtractTextFromPdf("document.pdf");
```

## 🔧 Configuration

### OCR Character Whitelist
The OCR engine is configured to recognize:
- English alphabet (A-Z, a-z)
- Romanian diacritics (ĂÂÎȘȚăâîșț)
- Numbers (0-9)
- Common punctuation (.,;:!?()-/\\ )

### Performance Settings
- **Image Resolution**: 300 DPI for optimal text recognition
- **OCR Engine Mode**: Default Tesseract engine mode
- **Language**: Romanian (`ron`)

## 🐛 Troubleshooting

### Common Issues

1. **DLL Not Found Error**
   ```
   System.DllNotFoundException: Unable to load DLL 'pdfium.dll'
   ```
   **Solution**: Add the PdfiumViewer native package:
   ```xml
   <PackageReference Include="PdfiumViewer.Native.x86_64.v8-xfa" Version="2018.4.8.256" />
   ```

2. **Tesseract Language Data Error**
   ```
   Error opening data file .\tessdata/ron.traineddata
   ```
   **Solution**: Ensure `ron.traineddata` is in the `tessdata` folder and the folder is copied to output directory.

3. **Input Directory Not Found**
   ```
   DirectoryNotFoundException: Could not find a part of the path
   ```
   **Solution**: Ensure the `input` directory exists and contains PDF files.

## 📊 Performance

- **Tested Scale**: Successfully processes 16,000+ PDF documents
- **Processing Mode**: Sequential processing with detailed logging
- **Memory Management**: Proper disposal of resources to handle large batches

## 🎯 Use Cases

- Document digitization projects
- Legal document processing
- Archive text extraction
- Romanian government document analysis
- Bulk PDF content indexing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is available under the MIT License. See the LICENSE file for more details.

## 🔗 Links

- [Repository](https://github.com/cosmincreato/proiectSenat)
- [Tesseract OCR](https://github.com/tesseract-ocr/tesseract)
- [PdfiumViewer Documentation](https://github.com/pvginkel/PdfiumViewer)

---

*Built with ❤️ for document processing and OCR analysis*