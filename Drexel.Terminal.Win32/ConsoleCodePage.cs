using System;

namespace Drexel.Terminal.Win32
{
    /// <summary>
    /// Represents a Windows code page. A code page is used by Windows to map bytes to displayable characters.
    /// </summary>
    // TODO: Need utility methods to convert these enum values to the .NET Name or official CP name for interop
    public enum ConsoleCodePage : uint
    {
        /// <summary>
        /// IBM EBCDIC US-Canada
        /// </summary>
        IBM037 = 037,

        /// <summary>
        /// OEM United States
        /// </summary>
        IBM437 = 437,

        /// <summary>
        /// IBM EBCDIC International
        /// </summary>
        IBM500 = 500,

        /// <summary>
        /// Arabic (ASMO 708)
        /// </summary>
        ASMO708 = 708,

        /// <summary>
        /// Arabic (ASMO-449+, BCON V4)
        /// </summary>
        CP709 = 709,

        /// <summary>
        /// Arabic - Transparent Arabic
        /// </summary>
        CP710 = 710,

        /// <summary>
        /// Arabic (Transparent ASMO); Arabic (DOS)
        /// </summary>
        DOS720 = 720,

        /// <summary>
        /// OEM Greek (formerly 437G); Greek (DOS)
        /// </summary>
        IBM737 = 737,

        /// <summary>
        /// OEM Baltic; Baltic (DOS)
        /// </summary>
        IBM775 = 775,

        /// <summary>
        /// OEM Multilingual Latin 1; Western European (DOS)
        /// </summary>
        IBM850 = 850,

        /// <summary>
        /// OEM Latin 2; Central European (DOS)
        /// </summary>
        IBM852 = 852,

        /// <summary>
        /// OEM Cyrillic (primarily Russian)
        /// </summary>
        IBM855 = 855,

        /// <summary>
        /// OEM Turkish; Turkish (DOS)
        /// </summary>
        IBM857 = 857,

        /// <summary>
        /// OEM Multilingual Latin 1 + Euro symbol
        /// </summary>
        IBM00858 = 858,

        /// <summary>
        /// OEM Portuguese; Portuguese (DOS)
        /// </summary>
        IBM860 = 860,

        /// <summary>
        /// OEM Icelandic; Icelandic (DOS)
        /// </summary>
        IBM861 = 861,

        /// <summary>
        /// OEM Hebrew; Hebrew (DOS)
        /// </summary>
        DOS862 = 862,

        /// <summary>
        /// OEM French Canadian; French Canadian (DOS)
        /// </summary>
        IBM863 = 863,

        /// <summary>
        /// OEM Arabic; Arabic (864)
        /// </summary>
        IBM864 = 864,

        /// <summary>
        /// OEM Nordic; Nordic (DOS)
        /// </summary>
        IBM865 = 865,

        /// <summary>
        /// OEM Russian; Cyrillic (DOS)
        /// </summary>
        CP866 = 866,

        /// <summary>
        /// OEM Modern Greek; Greek, Modern (DOS)
        /// </summary>
        ibm869 = 869,

        /// <summary>
        /// IBM EBCDIC Multilingual/ROECE (Latin 2); IBM EBCDIC Multilingual Latin 2
        /// </summary>
        IBM870 = 870,

        /// <summary>
        /// ANSI/OEM Thai (ISO 8859-11); Thai (Windows)
        /// </summary>
        Windows874 = 874,

        /// <summary>
        /// IBM EBCDIC Greek Modern
        /// </summary>
        CP875 = 875,

        /// <summary>
        /// ANSI/OEM Japanese; Japanese (Shift-JIS)
        /// </summary>
        ShiftJis = 932,

        /// <summary>
        /// ANSI/OEM Simplified Chinese (PRC, Singapore); Chinese Simplified (GB2312)
        /// </summary>
        GB2312 = 936,

        /// <summary>
        /// ANSI/OEM Korean (Unified Hangul Code)
        /// </summary>
        KsC5601_1987 = 949,

        /// <summary>
        /// ANSI/OEM Traditional Chinese (Taiwan; Hong Kong SAR, PRC); Chinese Traditional (Big5)
        /// </summary>
        Big5 = 950,

        /// <summary>
        /// IBM EBCDIC Turkish (Latin 5)
        /// </summary>
        IBM1026 = 1026,

        /// <summary>
        /// IBM EBCDIC Latin 1/Open System
        /// </summary>
        IBM01047 = 1047,

        /// <summary>
        /// IBM EBCDIC US-Canada (037 + Euro symbol); IBM EBCDIC (US-Canada-Euro)
        /// </summary>
        IBM01140 = 1140,

        /// <summary>
        /// IBM EBCDIC Germany (20273 + Euro symbol); IBM EBCDIC (Germany-Euro)
        /// </summary>
        IBM01141 = 1141,

        /// <summary>
        /// IBM EBCDIC Denmark-Norway (20277 + Euro symbol); IBM EBCDIC (Denmark-Norway-Euro)
        /// </summary>
        IBM01142 = 1142,

        /// <summary>
        /// IBM EBCDIC Finland-Sweden (20278 + Euro symbol); IBM EBCDIC (Finland-Sweden-Euro)
        /// </summary>
        IBM01143 = 1143,

        /// <summary>
        /// IBM EBCDIC Italy (20280 + Euro symbol); IBM EBCDIC (Italy-Euro)
        /// </summary>
        IBM01144 = 1144,

        /// <summary>
        /// IBM EBCDIC Latin America-Spain (20284 + Euro symbol); IBM EBCDIC (Spain-Euro)
        /// </summary>
        IBM01145 = 1145,

        /// <summary>
        /// IBM EBCDIC United Kingdom (20285 + Euro symbol); IBM EBCDIC (UK-Euro)
        /// </summary>
        IBM01146 = 1146,

        /// <summary>
        /// IBM EBCDIC France (20297 + Euro symbol); IBM EBCDIC (France-Euro)
        /// </summary>
        IBM01147 = 1147,

        /// <summary>
        /// IBM EBCDIC International (500 + Euro symbol); IBM EBCDIC (International-Euro)
        /// </summary>
        IBM01148 = 1148,

        /// <summary>
        /// IBM EBCDIC Icelandic (20871 + Euro symbol); IBM EBCDIC (Icelandic-Euro)
        /// </summary>
        IBM01149 = 1149,

        /// <summary>
        /// Unicode UTF-16, little endian byte order (BMP of ISO 10646); available only to managed applications
        /// </summary>
        Utf16 = 1200,

        /// <summary>
        /// Unicode UTF-16, big endian byte order; available only to managed applications
        /// </summary>
        UnicodeFFFE = 1201,

        /// <summary>
        /// ANSI Central European; Central European (Windows)
        /// </summary>
        Windows1250 = 1250,

        /// <summary>
        /// ANSI Cyrillic; Cyrillic (Windows)
        /// </summary>
        Windows1251 = 1251,

        /// <summary>
        /// ANSI Latin 1; Western European (Windows)
        /// </summary>
        Windows1252 = 1252,

        /// <summary>
        /// ANSI Greek; Greek (Windows)
        /// </summary>
        Windows1253 = 1253,

        /// <summary>
        /// ANSI Turkish; Turkish (Windows)
        /// </summary>
        Windows1254 = 1254,

        /// <summary>
        /// ANSI Hebrew; Hebrew (Windows)
        /// </summary>
        Windows1255 = 1255,

        /// <summary>
        /// ANSI Arabic; Arabic (Windows)
        /// </summary>
        Windows1256 = 1256,

        /// <summary>
        /// ANSI Baltic; Baltic (Windows)
        /// </summary>
        Windows1257 = 1257,

        /// <summary>
        /// ANSI/OEM Vietnamese; Vietnamese (Windows)
        /// </summary>
        Windows1258 = 1258,

        /// <summary>
        /// Korean (Johab)
        /// </summary>
        Johab = 1361,

        /// <summary>
        /// MAC Roman; Western European (Mac)
        /// </summary>
        MacWesternEuropean = 10000,

        /// <summary>
        /// Japanese (Mac)
        /// </summary>
        MacJapanese = 10001,

        /// <summary>
        /// MAC Traditional Chinese (Big5); Chinese Traditional (Mac)
        /// </summary>
        MacChineseTraditional = 10002,

        /// <summary>
        /// Korean (Mac)
        /// </summary>
        MacKorean = 10003,

        /// <summary>
        /// Arabic (Mac)
        /// </summary>
        MacArabic = 10004,

        /// <summary>
        /// Hebrew (Mac)
        /// </summary>
        MacHebrew = 10005,

        /// <summary>
        /// Greek (Mac)
        /// </summary>
        MacGreek = 10006,

        /// <summary>
        /// Cyrillic (Mac)
        /// </summary>
        MacCyrillic = 10007,

        /// <summary>
        /// MAC Simplified Chinese (GB 2312); Chinese Simplified (Mac)
        /// </summary>
        MacChineseSimplified = 10008,

        /// <summary>
        /// Romanian (Mac)
        /// </summary>
        MacRomanian = 10010,

        /// <summary>
        /// Ukrainian (Mac)
        /// </summary>
        MacUkrainian = 10017,

        /// <summary>
        /// Thai (Mac)
        /// </summary>
        MacThai = 10021,

        /// <summary>
        /// MAC Latin 2; Central European (Mac)
        /// </summary>
        MacCentralEuropean = 10029,

        /// <summary>
        /// Icelandic (Mac)
        /// </summary>
        MacIcelandic = 10079,

        /// <summary>
        /// Turkish (Mac)
        /// </summary>
        MacTurkish = 10081,

        /// <summary>
        /// Croatian (Mac)
        /// </summary>
        MacCroatian = 10082,

        /// <summary>
        /// Unicode UTF-32, little endian byte order; available only to managed applications
        /// </summary>
        Utf32 = 12000,

        /// <summary>
        /// Unicode UTF-32, big endian byte order; available only to managed applications
        /// </summary>
        Utf32BE = 12001,

        /// <summary>
        /// CNS Taiwan; Chinese Traditional (CNS)
        /// </summary>
        CNSChineseTraditional = 20000,

        /// <summary>
        /// TCA Taiwan
        /// </summary>
        TCATaiwan = 20001,

        /// <summary>
        /// Eten Taiwan; Chinese Traditional (Eten)
        /// </summary>
        EtenChineseTraditional = 20002,

        /// <summary>
        /// IBM5550 Taiwan
        /// </summary>
        IBM5550 = 20003,

        /// <summary>
        /// TeleText Taiwan
        /// </summary>
        TeleTextTaiwan = 20004,

        /// <summary>
        /// Wang Taiwan
        /// </summary>
        WangTaiwan = 20005,

        /// <summary>
        /// IA5 (IRV International Alphabet No. 5, 7-bit); Western European (IA5)
        /// </summary>
        IA5WesternEuropean = 20105,

        /// <summary>
        /// IA5 German (7-bit)
        /// </summary>
        IA5German = 20106,

        /// <summary>
        /// IA5 Swedish (7-bit)
        /// </summary>
        IA5Swedish = 20107,

        /// <summary>
        /// IA5 Norwegian (7-bit)
        /// </summary>
        IA5Norwegian = 20108,

        /// <summary>
        /// US-ASCII (7-bit)
        /// </summary>
        USAscii = 20127,

        /// <summary>
        /// T.61
        /// </summary>
        ITUT61 = 20261,

        /// <summary>
        /// ISO 6937 Non-Spacing Accent
        /// </summary>
        CP20269 = 20269,

        /// <summary>
        /// IBM EBCDIC Germany
        /// </summary>
        IBM273 = 20273,

        /// <summary>
        /// IBM EBCDIC Denmark-Norway
        /// </summary>
        IBM277 = 20277,

        /// <summary>
        /// IBM EBCDIC Finland-Sweden
        /// </summary>
        IBM278 = 20278,

        /// <summary>
        /// IBM EBCDIC Italy
        /// </summary>
        IBM280 = 20280,

        /// <summary>
        /// IBM EBCDIC Latin America-Spain
        /// </summary>
        IBM284 = 20284,

        /// <summary>
        /// IBM EBCDIC United Kingdom
        /// </summary>
        IBM285 = 20285,

        /// <summary>
        /// IBM EBCDIC Japanese Katakana Extended
        /// </summary>
        IBM290 = 20290,

        /// <summary>
        /// IBM EBCDIC France
        /// </summary>
        IBM297 = 20297,

        /// <summary>
        /// IBM EBCDIC Arabic
        /// </summary>
        IBM420 = 20420,

        /// <summary>
        /// IBM EBCDIC Greek
        /// </summary>
        IBM423 = 20423,

        /// <summary>
        /// IBM EBCDIC Hebrew
        /// </summary>
        IBM424 = 20424,

        /// <summary>
        /// IBM EBCDIC Korean Extended
        /// </summary>
        IBM13121 = 20833,

        /// <summary>
        /// IBM EBCDIC Thai
        /// </summary>
        IBM1160 = 20838,

        /// <summary>
        /// Russian (KOI8-R); Cyrillic (KOI8-R)
        /// </summary>
        KOI8R = 20866,

        /// <summary>
        /// IBM EBCDIC Icelandic
        /// </summary>
        IBM871 = 20871,

        /// <summary>
        /// IBM EBCDIC Cyrillic Russian
        /// </summary>
        IBM880 = 20880,

        /// <summary>
        /// IBM EBCDIC Turkish
        /// </summary>
        IBM905 = 20905,

        /// <summary>
        /// IBM EBCDIC Latin 1/Open System (1047 + Euro symbol)
        /// </summary>
        IBM924 = 20924,

        /// <summary>
        /// Japanese (JIS 0208-1990 and 0212-1990). This code page is functionally identical to <see cref="EUC_JP"/>;
        /// when a managed application uses this code page, it will actually use <see cref="EUC_JP"/>. However,
        /// <see cref="EUC_JP"/> is available only to managed applications, so unmanaged (native) applications use this
        /// code page instead.
        /// </summary>
        EucJP_NativeApplications = 20932,

        /// <summary>
        /// Simplified Chinese (GB2312); Chinese Simplified (GB2312-80)
        /// </summary>
        GB2312_80 = 20936,

        /// <summary>
        /// Korean Wansung
        /// </summary>
        CP20949 = 20949,

        /// <summary>
        /// IBM EBCDIC Cyrillic Serbian-Bulgarian
        /// </summary>
        CP21025 = 21025,

        /// <summary>
        /// Ext Alpha Lowercase
        /// </summary>
        [Obsolete]
        CP21027 = 21027,

        /// <summary>
        /// Ukrainian (KOI8-U); Cyrillic (KOI8-U)
        /// </summary>
        KOI8U = 21866,

        /// <summary>
        /// ISO 8859-1 Latin 1; Western European (ISO)
        /// </summary>
        ISO8859_1 = 28591,

        /// <summary>
        /// ISO 8859-2 Central European; Central European (ISO)
        /// </summary>
        ISO8859_2 = 28592,

        /// <summary>
        /// ISO 8859-3 Latin 3
        /// </summary>
        ISO8859_3 = 28593,

        /// <summary>
        /// ISO 8859-4 Baltic
        /// </summary>
        ISO8859_4 = 28594,

        /// <summary>
        /// ISO 8859-5 Cyrillic
        /// </summary>
        ISO8859_5 = 28595,

        /// <summary>
        /// ISO 8859-6 Arabic
        /// </summary>
        ISO8859_6 = 28596,

        /// <summary>
        /// ISO 8859-7 Greek
        /// </summary>
        ISO8859_7 = 28597,

        /// <summary>
        /// ISO 8859-8 Hebrew; Hebrew (ISO-Visual)
        /// </summary>
        ISO8859_8 = 28598,

        /// <summary>
        /// ISO 8859-9 Turkish
        /// </summary>
        ISO8859_9 = 28599,

        /// <summary>
        /// ISO 8859-13 Estonian
        /// </summary>
        ISO8859_13 = 28603,

        /// <summary>
        /// ISO 8859-15 Latin 9
        /// </summary>
        ISO8859_15 = 28605,

        /// <summary>
        /// Europa 3
        /// </summary>
        Europa = 29001,

        /// <summary>
        /// ISO 8859-8 Hebrew; Hebrew (ISO-Logical)
        /// </summary>
        ISO8859_8_I = 38598,

        /// <summary>
        /// ISO 2022 Japanese with no halfwidth Katakana; Japanese (JIS)
        /// </summary>
        ISO2022_JP = 50220,

        /// <summary>
        /// ISO 2022 Japanese with halfwidth Katakana; Japanese (JIS-Allow 1 byte Kana)
        /// </summary>
        ISO2022_JP_EXT = 50221,

        /// <summary>
        /// ISO 2022 Japanese JIS X 0201-1989; Japanese (JIS-Allow 1 byte Kana - SO/SI)
        /// </summary>
        ISO2022_JP_SIO = 50222,

        /// <summary>
        /// ISO 2022 Korean
        /// </summary>
        ISO2022_KR = 50225,

        /// <summary>
        /// ISO 2022 Simplified Chinese; Chinese Simplified (ISO 2022)
        /// </summary>
        ISO2022_CN_Simplified = 50227,

        /// <summary>
        /// ISO 2022 Traditional Chinese
        /// </summary>
        ISO2022_CN_Traditional = 50229,

        /// <summary>
        /// EBCDIC Japanese (Katakana) Extended
        /// </summary>
        CP50930 = 50930,

        /// <summary>
        /// EBCDIC US-Canada and Japanese
        /// </summary>
        CP50931 = 50931,

        /// <summary>
        /// EBCDIC Korean Extended and Korean
        /// </summary>
        CP50933 = 50933,

        /// <summary>
        /// EBCDIC Simplified Chinese Extended and Simplified Chinese
        /// </summary>
        CP50935 = 50935,

        /// <summary>
        /// EBCDIC Simplified Chinese
        /// </summary>
        CP50936 = 50936,

        /// <summary>
        /// EBCDIC US-Canada and Traditional Chinese
        /// </summary>
        CP50937 = 50937,

        /// <summary>
        /// EBCDIC Japanese (Latin) Extended and Japanese
        /// </summary>
        CP50939 = 50939,

        /// <summary>
        /// EUC Japanese
        /// </summary>
        EUC_JP = 51932,

        /// <summary>
        /// EUC Simplified Chinese; Chinese Simplified (EUC)
        /// </summary>
        EUC_CN_Simplified = 51936,

        /// <summary>
        /// EUC Korean
        /// </summary>
        EUC_KR = 51949,

        /// <summary>
        /// EUC Traditional Chinese
        /// </summary>
        EUC_CN_Traditional = 51950,

        /// <summary>
        /// HZ-GB2312 Simplified Chinese; Chinese Simplified (HZ)
        /// </summary>
        GB2312_HZ = 52936,

        /// <summary>
        /// Windows XP and later: GB18030 Simplified Chinese (4 byte); Chinese Simplified (GB18030)
        /// </summary>
        GB18030 = 54936,

        /// <summary>
        /// ISCII Devanagari
        /// </summary>
        IsciiDe = 57002,

        /// <summary>
        /// ISCII Bangla
        /// </summary>
        IsciiBe = 57003,

        /// <summary>
        /// ISCII Tamil
        /// </summary>
        IsciiTa = 57004,

        /// <summary>
        /// ISCII Telugu
        /// </summary>
        IsciiTe = 57005,

        /// <summary>
        /// ISCII Assamese
        /// </summary>
        IsciiAs = 57006,

        /// <summary>
        /// ISCII Odia
        /// </summary>
        IsciiOd = 57007,

        /// <summary>
        /// ISCII Kannada
        /// </summary>
        IsciiKa = 57008,

        /// <summary>
        /// ISCII Malayalam
        /// </summary>
        IsciiMa = 57009,

        /// <summary>
        /// ISCII Gujarati
        /// </summary>
        IsciiGu = 57010,

        /// <summary>
        /// ISCII Punjabi
        /// </summary>
        IsciiPa = 57011,

        /// <summary>
        /// Unicode (UTF-7)
        /// </summary>
        Utf7 = 65000,

        /// <summary>
        /// Unicode (UTF-8)
        /// </summary>
        Utf8 = 65001
    }
}
