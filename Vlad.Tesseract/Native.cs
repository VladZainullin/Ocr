using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

/// <summary>
/// Предоставляет низкоуровневый доступ к нативной библиотеке Tesseract 5.0
/// </summary>
internal static class Native
{
    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";

    /// <summary>
    /// Создает новый экземпляр базового API Tesseract
    /// </summary>
    /// <returns>Указатель на созданный экземпляр</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPICreate();

    /// <summary>
    /// Удаляет экземпляр базового API Tesseract и освобождает ресурсы
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIDelete(IntPtr handle);

    /// <summary>
    /// Устанавливает режим сегментации страницы.
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="mode">Режим сегментации (значение от 0 до 14)</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetPageSegMode(IntPtr handle, int mode);

    /// <summary>
    /// Инициализирует Tesseract с указанным путем к данным и языком
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="dataPath">Путь к папке tessdata</param>
    /// <param name="language">Язык (например, "rus+eng")</param>
    /// <returns>0 при успехе, -1 при ошибке</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false,
        ThrowOnUnmappableChar = true)]
    public static extern int TessBaseAPIInit3(IntPtr handle,
        [MarshalAs(UnmanagedType.LPStr)] string dataPath,
        [MarshalAs(UnmanagedType.LPStr)] string language);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
    public static extern IntPtr ResultIteratorGetUTF8TextInternal(IntPtr handle, PageIteratorLevel level);

    /// <summary>
    /// Устанавливает изображение для распознавания из данных пикселей
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="imagedata">Указатель на данные изображения</param>
    /// <param name="width">Ширина изображения</param>
    /// <param name="height">Высота изображения</param>
    /// <param name="bytesPerPixel">Байт на пиксель</param>
    /// <param name="bytesPerLine">Байт на строку</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetImage(IntPtr handle, IntPtr imagedata, uint width, uint height,
        uint bytesPerPixel, uint bytesPerLine);

    /// <summary>
    /// Выполняет распознавание на установленном изображении
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="monitor"></param>
    /// <returns>0 при успехе, -1 при ошибке</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int TessBaseAPIRecognize(IntPtr handle, IntPtr monitor);

    /// <summary>
    /// Выполняет распознавание и возвращает UTF-8 текст
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <returns>Указатель на строку с текстом</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);

    /// <summary>
    /// Освобождает память, выделенную Tesseract
    /// </summary>
    /// <param name="text">Указатель на текст для освобождения</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteText(IntPtr text);

    /// <summary>
    /// Получает итератор страницы после распознавания
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <returns>Указатель на итератор результата</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetIterator(IntPtr handle);

    /// <summary>
    /// Перемещает итератор к следующему элементу
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    /// <param name="level">Уровень итерации</param>
    /// <returns>true если есть следующий элемент</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool TessPageIteratorNext(IntPtr iterator, PageIteratorLevel level);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
    public static extern int PageIteratorIsAtFinalElement(IntPtr handle, PageIteratorLevel level,
        PageIteratorLevel element);
}