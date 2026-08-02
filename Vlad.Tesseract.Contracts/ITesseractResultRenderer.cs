namespace Vlad.Tesseract.Contracts;

public interface ITesseractResultRenderer
{
    string Extension { get; }
    
    string Title { get; }
    
    int ImageNumbers { get; }
    
    bool TryNext();
    
    void Insert(ITesseractResultRenderer renderer);

    bool TryBeginDocument(string title);

    bool TryAddImage(ITesseractEngine engine);
    
    bool TryEndDocument();
}