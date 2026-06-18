using System.Runtime.InteropServices;

namespace Vlad.Tesseract;

/// <summary>
/// Предоставляет низкоуровневый доступ к нативной библиотеке Tesseract 5.0
/// </summary>
internal static class Native
{
    #region Константы и настройки

    private const string DllName = @"C:\Users\user\RiderProjects\Ocr\Web\bin\Debug\net10.0\x64\tesseract50.dll";

    // Константы для страниц
    public const int PageIteratorLevelBlock = 0;
    public const int PageIteratorLevelPara = 1;
    public const int PageIteratorLevelTextLine = 2;
    public const int PageIteratorLevelWord = 3;
    public const int PageIteratorLevelSymbol = 4;

    // Константы для Orientation
    public const int OrientationPageUp = 0;
    public const int OrientationPageRight = 1;
    public const int OrientationPageDown = 2;
    public const int OrientationPageLeft = 3;

    #endregion

    #region Базовые методы управления API

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
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    public static extern int TessBaseAPIInit3(IntPtr handle, 
        [MarshalAs(UnmanagedType.LPStr)] string dataPath, 
        [MarshalAs(UnmanagedType.LPStr)] string language);

    /// <summary>
    /// Инициализирует Tesseract с расширенными параметрами
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="dataPath">Путь к папке tessdata</param>
    /// <param name="language">Язык</param>
    /// <param name="mode">Режим распознавания (OEM)</param>
    /// <param name="configs">Массив конфигурационных файлов</param>
    /// <param name="configsSize">Размер массива конфигураций</param>
    /// <returns>0 при успехе, -1 при ошибке</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int TessBaseAPIInit4(IntPtr handle, string dataPath, string language, 
        int mode, string[] configs, int configsSize);

    /// <summary>
    /// Инициализирует Tesseract для использования только языка
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="language">Язык</param>
    /// <returns>0 при успехе, -1 при ошибке</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int TessBaseAPIInitLang(IntPtr handle, string language);

    /// <summary>
    /// Возвращает версию Tesseract
    /// </summary>
    /// <returns>Строка с версией</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessVersion();

    #endregion

    #region Управление изображениями

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
    /// Устанавливает изображение из 2D массива пикселей
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="imagedata">Указатель на данные изображения</param>
    /// <param name="width">Ширина</param>
    /// <param name="height">Высота</param>
    /// <param name="bytesPerPixel">Байт на пиксель</param>
    /// <param name="bytesPerLine">Байт на строку</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetImage2D(IntPtr handle, IntPtr imagedata, int width, int height, 
        int bytesPerPixel, int bytesPerLine);

    /// <summary>
    /// Устанавливает изображение из файла
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="imagePath">Путь к файлу изображения</param>
    /// <returns>0 при успехе, -1 при ошибке</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int TessBaseAPISetImageFile(IntPtr handle, string imagePath);

    /// <summary>
    /// Устанавливает прямоугольную область интереса на изображении
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="left">Левая координата</param>
    /// <param name="top">Верхняя координата</param>
    /// <param name="width">Ширина области</param>
    /// <param name="height">Высота области</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetRectangle(IntPtr handle, int left, int top, int width, int height);

    /// <summary>
    /// Очищает изображение и освобождает связанные ресурсы
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIClear(IntPtr handle);

    /// <summary>
    /// Очищает все внутренние буферы
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIClearPersistentCache(IntPtr handle);

    #endregion

    #region Распознавание текста

    /// <summary>
    /// Выполняет распознавание и возвращает UTF-8 текст
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <returns>Указатель на строку с текстом</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetUTF8Text(IntPtr handle);

    /// <summary>
    /// Выполняет распознавание и возвращает HOCR формат
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на HOCR строку</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetHOCRText(IntPtr handle, int pageNumber);

    /// <summary>
    /// Выполняет распознавание и возвращает ALTO XML формат
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на ALTO XML строку</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetAltoText(IntPtr handle, int pageNumber);

    /// <summary>
    /// Выполняет распознавание и возвращает TSV формат
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на TSV строку</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetTSVText(IntPtr handle, int pageNumber);

    /// <summary>
    /// Выполняет распознавание и возвращает Box формат
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на Box строку</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetBoxText(IntPtr handle, int pageNumber);

    /// <summary>
    /// Выполняет распознавание и возвращает UNLV формат
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на UNLV строку</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetUNLVText(IntPtr handle, int pageNumber);

    /// <summary>
    /// Получает распознанный текст с информацией о достоверности
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="text">Выходной текст</param>
    /// <param name="confidences">Выходной массив достоверностей</param>
    /// <param name="lengths">Выходной массив длин</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIGetTextAndConfidences(IntPtr handle, out IntPtr text, out IntPtr confidences, out IntPtr lengths);

    #endregion

    #region Управление памятью

    /// <summary>
    /// Освобождает память, выделенную Tesseract
    /// </summary>
    /// <param name="text">Указатель на текст для освобождения</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteText(IntPtr text);

    /// <summary>
    /// Освобождает память массива строк
    /// </summary>
    /// <param name="texts">Указатель на массив строк</param>
    /// <param name="count">Количество строк</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteTexts(IntPtr texts, int count);

    /// <summary>
    /// Освобождает память массива целых чисел
    /// </summary>
    /// <param name="array">Указатель на массив</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteIntArray(IntPtr array);

    #endregion

    #region Итераторы и анализ страницы

    /// <summary>
    /// Создает итератор страницы для анализа структуры
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <returns>Указатель на итератор страницы</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIAnalysePage(IntPtr handle);

    /// <summary>
    /// Получает итератор страницы после распознавания
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <returns>Указатель на итератор результата</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessBaseAPIGetIterator(IntPtr handle);

    /// <summary>
    /// Удаляет итератор страницы
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessPageIteratorDelete(IntPtr iterator);

    /// <summary>
    /// Перемещает итератор к следующему элементу
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    /// <param name="level">Уровень итерации</param>
    /// <returns>true если есть следующий элемент</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool TessPageIteratorNext(IntPtr iterator, int level);

    /// <summary>
    /// Проверяет, является ли текущий элемент началом
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    /// <param name="level">Уровень итерации</param>
    /// <returns>true если это начало</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool TessPageIteratorIsAtBeginningOf(IntPtr iterator, int level);

    /// <summary>
    /// Получает границы текущего элемента
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    /// <param name="level">Уровень итерации</param>
    /// <param name="left">Левая граница</param>
    /// <param name="top">Верхняя граница</param>
    /// <param name="right">Правая граница</param>
    /// <param name="bottom">Нижняя граница</param>
    /// <returns>true при успехе</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool TessPageIteratorBoundingBox(IntPtr iterator, int level, 
        out int left, out int top, out int right, out int bottom);

    /// <summary>
    /// Получает текст текущего элемента
    /// </summary>
    /// <param name="iterator">Указатель на итератор</param>
    /// <param name="level">Уровень итерации</param>
    /// <returns>Указатель на текст</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr TessPageIteratorGetUTF8Text(IntPtr iterator, int level);

    #endregion

    #region Настройки и параметры

    /// <summary>
    /// Устанавливает переменную в Tesseract
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="name">Имя переменной</param>
    /// <param name="value">Значение</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void TessBaseAPISetVariable(IntPtr handle, string name, string value);

    /// <summary>
    /// Получает значение переменной
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="name">Имя переменной</param>
    /// <param name="value">Выходное значение</param>
    /// <returns>true если переменная существует</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool TessBaseAPIGetVariable(IntPtr handle, string name, out string value);

    /// <summary>
    /// Устанавливает параметры распознавания из файла
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="configFilePath">Путь к файлу конфигурации</param>
    /// <returns>0 при успехе</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int TessBaseAPIReadConfigFile(IntPtr handle, string configFilePath);

    /// <summary>
    /// Устанавливает отладочный уровень
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="debugLevel">Уровень отладки</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPISetDebugLevel(IntPtr handle, int debugLevel);

    #endregion

    #region Обработка и ориентация

    /// <summary>
    /// Определяет ориентацию страницы и письменность
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="orientation">Ориентация страницы</param>
    /// <param name="writingDirection">Направление письма</param>
    /// <param name="textLineOrder">Порядок строк</param>
    /// <param name="deskewAngle">Угол перекоса</param>
    /// <returns>0 при успехе</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int TessBaseAPIDetectOrientation(IntPtr handle, out int orientation, 
        out int writingDirection, out int textLineOrder, out float deskewAngle);

    /// <summary>
    /// Определяет перекос изображения
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="deskewAngle">Угол перекоса</param>
    /// <returns>0 при успехе</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int TessBaseAPIDetectOS(IntPtr handle, out float deskewAngle);

    /// <summary>
    /// Распознает и исправляет ориентацию страницы
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="orientation">Ориентация</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessBaseAPIRecognizePage(IntPtr handle, out int orientation);

    #endregion

    #region Управление словарями

    /// <summary>
    /// Загружает пользовательский словарь
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="dictionaryPath">Путь к файлу словаря</param>
    /// <returns>0 при успехе</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int TessBaseAPILoadUserDict(IntPtr handle, string dictionaryPath);

    /// <summary>
    /// Добавляет слова в пользовательский словарь
    /// </summary>
    /// <param name="handle">Указатель на экземпляр API</param>
    /// <param name="word">Слово для добавления</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void TessBaseAPIAddWord(IntPtr handle, string word);

    #endregion

    #region Рендеринг

    /// <summary>
    /// Создает рендерер для HOCR формата
    /// </summary>
    /// <param name="outputPrefix">Префикс выходного файла</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на рендерер</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessHOCRCreate(string outputPrefix, int pageNumber);

    /// <summary>
    /// Создает рендерер для ALTO формата
    /// </summary>
    /// <param name="outputPrefix">Префикс выходного файла</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на рендерер</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessAltoCreate(string outputPrefix, int pageNumber);

    /// <summary>
    /// Создает рендерер для TSV формата
    /// </summary>
    /// <param name="outputPrefix">Префикс выходного файла</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на рендерер</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessTSVCreate(string outputPrefix, int pageNumber);

    /// <summary>
    /// Создает рендерер для Box формата
    /// </summary>
    /// <param name="outputPrefix">Префикс выходного файла</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на рендерер</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessBoxCreate(string outputPrefix, int pageNumber);

    /// <summary>
    /// Создает рендерер для UNLV формата
    /// </summary>
    /// <param name="outputPrefix">Префикс выходного файла</param>
    /// <param name="pageNumber">Номер страницы</param>
    /// <returns>Указатель на рендерер</returns>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr TessUNLVCreate(string outputPrefix, int pageNumber);

    /// <summary>
    /// Удаляет рендерер
    /// </summary>
    /// <param name="renderer">Указатель на рендерер</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessDeleteRenderer(IntPtr renderer);

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Освобождает память, выделенную Tesseract
    /// </summary>
    /// <param name="pointer">Указатель на память</param>
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void TessFree(IntPtr pointer);

    #endregion
}