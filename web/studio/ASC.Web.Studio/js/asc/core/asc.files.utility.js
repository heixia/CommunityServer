/*
 *
 * (c) Copyright Ascensio System Limited 2010-2016
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


if (typeof ASC === 'undefined') {
    ASC = {};
}
if (typeof ASC.Files === 'undefined') {
    ASC.Files = (function () {
        return {};
    })();
}
if (typeof ASC.Files.Utility === 'undefined') {
    ASC.Files.Utility = {};
}

ASC.Files.Utility.FileExtensionLibrary = {
    ArchiveExts: [".zip", ".rar", ".ace", ".arc", ".arj", ".bh", ".cab", ".enc", ".gz", ".ha", ".jar", ".lha", ".lzh", ".pak", ".pk3", ".tar", ".tgz", ".uu", ".uue", ".xxe", ".z", ".zoo"],
    AviExts: [".avi"],
    CsvExts: [".csv"],
    DjvuExts: [".djvu"],
    DocExts: [".doc"],
    DocxExts: [".docx"],
    DoctExts: [".doct"],
    EbookExts: [".epub", ".fb2"],
    FlvExts: [".flv", ".fla"],
    HtmlExts: [".html", ".htm", ".mht"],
    IafExts: [".iaf"],
    ImgExts: [".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg", ".jfif", ".tiff", ".tif", ".cmx", ".ico", ".png", ".pnm", ".pbm", ".ppm", ".rgb", ".xbm", ".xpm", ".xwd"],
    M2tsExts: [".m2ts"],
    MkvExts: [".mkv"],
    MovExts: [".mov"],
    Mp4Exts: [".mp4"],
    MpgExts: [".mpg"],
    OdpExts: [".odp"],
    OdsExts: [".ods"],
    OdtExts: [".odt"],
    PdfExts: [".pdf"],
    PpsExts: [".pps"],
    PpsxExts: [".ppsx"],
    PptExts: [".ppt"],
    PptxExts: [".pptx"],
    PpttExts: [".pptt"],
    RtfExts: [".rtf"],
    SoundExts: [".mp3", ".wav", ".pcm", ".aiff", ".3gp", ".flac", ".fla", ".cda"],
    SvgExts: [".svg"],
    TxtExts: [".txt"],
    DvdExts: [".vob"],
    XlsExts: [".xls"],
    XlsxExts: [".xlsx"],
    XlstExts: [".xlst"],
    XmlExts: [".xml"],
    XpsExts: [".xps"],
    GdocExts: [".gdoc"],
    GsheetExts: [".gsheet"],
    GslidesExts: [".gslides"],
    CalendarExts: [".ics", ".ical", ".ifb", ".icalendar"]
};

ASC.Files.Utility.getCssClassByFileTitle = function (fileTitle, compact) {
    var utility = ASC.Files.Utility,
        fileExtensionLibrary = utility.FileExtensionLibrary,
        fileExt = utility.GetFileExtension(fileTitle),
        ext = "file",
        checkInArray = function(extsArray) {
            return jq.inArray(fileExt, extsArray) != -1;
        };


    if (checkInArray(fileExtensionLibrary.ArchiveExts))
        ext = "Archive";
    else if (checkInArray(fileExtensionLibrary.AviExts))
        ext = "Avi";
    else if (checkInArray(fileExtensionLibrary.CsvExts))
        ext = "Csv";
    else if (checkInArray(fileExtensionLibrary.DjvuExts))
        ext = "Djvu";
    else if (checkInArray(fileExtensionLibrary.DocExts))
        ext = "Doc";
    else if (checkInArray(fileExtensionLibrary.DocxExts))
        ext = "Docx";
    else if (checkInArray(fileExtensionLibrary.DoctExts))
        ext = "Doct";
    else if (checkInArray(fileExtensionLibrary.EbookExts))
        ext = "Ebook";
    else if (checkInArray(fileExtensionLibrary.FlvExts))
        ext = "Flv";
    else if (checkInArray(fileExtensionLibrary.HtmlExts))
        ext = "Html";
    else if (checkInArray(fileExtensionLibrary.IafExts))
        ext = "Iaf";
    else if (checkInArray(fileExtensionLibrary.ImgExts))
        ext = "Image";
    else if (checkInArray(fileExtensionLibrary.M2tsExts))
        ext = "M2ts";
    else if (checkInArray(fileExtensionLibrary.MkvExts))
        ext = "Mkv";
    else if (checkInArray(fileExtensionLibrary.MovExts))
        ext = "Mov";
    else if (checkInArray(fileExtensionLibrary.Mp4Exts))
        ext = "Mp4";
    else if (checkInArray(fileExtensionLibrary.MpgExts))
        ext = "Mpg";
    else if (checkInArray(fileExtensionLibrary.OdpExts))
        ext = "Odp";
    else if (checkInArray(fileExtensionLibrary.OdtExts))
        ext = "Odt";
    else if (checkInArray(fileExtensionLibrary.OdsExts))
        ext = "Ods";
    else if (checkInArray(fileExtensionLibrary.PdfExts))
        ext = "Pdf";
    else if (checkInArray(fileExtensionLibrary.PpsExts))
        ext = "Pps";
    else if (checkInArray(fileExtensionLibrary.PpsxExts))
        ext = "Ppsx";
    else if (checkInArray(fileExtensionLibrary.PptExts))
        ext = "Ppt";
    else if (checkInArray(fileExtensionLibrary.PptxExts))
        ext = "Pptx";
    else if (checkInArray(fileExtensionLibrary.PpttExts))
        ext = "Pptt";
    else if (checkInArray(fileExtensionLibrary.RtfExts))
        ext = "Rtf";
    else if (checkInArray(fileExtensionLibrary.SoundExts))
        ext = "Sound";
    else if (checkInArray(fileExtensionLibrary.SvgExts))
        ext = "Svg";
    else if (checkInArray(fileExtensionLibrary.TxtExts))
        ext = "Txt";
    else if (checkInArray(fileExtensionLibrary.DvdExts))
        ext = "Dvd";
    else if (checkInArray(fileExtensionLibrary.XlsExts))
        ext = "Xls";
    else if (checkInArray(fileExtensionLibrary.XlsxExts))
        ext = "Xlsx";
    else if (checkInArray(fileExtensionLibrary.XlstExts))
        ext = "Xlst";
    else if (checkInArray(fileExtensionLibrary.XmlExts))
        ext = "Xml";
    else if (checkInArray(fileExtensionLibrary.XpsExts))
        ext = "Xps";
    else if (checkInArray(fileExtensionLibrary.GdocExts))
        ext = "Gdoc";
    else if (checkInArray(fileExtensionLibrary.GsheetExts))
        ext = "Gsheet";
    else if (checkInArray(fileExtensionLibrary.GslidesExts))
        ext = "Gslides";
    else if (checkInArray(fileExtensionLibrary.CalendarExts))
        ext = "Cal";

    return "ftFile_" + (compact === true ? 21 : 32) + " ft_" + ext;
};

ASC.Files.Utility.getFolderCssClass = function (compact) {
    return "ftFolder_" + (compact === true ? 21 : 32);
};

ASC.Files.Utility.CanWebEditBrowser =
    true === (jq.browser.msie && jq.browser.versionCorrect >= 9
        || jq.browser.safari && jq.browser.versionCorrect >= 5
        || jq.browser.mozilla && jq.browser.versionCorrect >= 4
        || jq.browser.chrome && jq.browser.versionCorrect >= 7
        || jq.browser.opera && jq.browser.versionCorrect >= 10.5);

ASC.Files.Utility.GetFileExtension = function (fileTitle) {
    if (typeof fileTitle == "undefined" || fileTitle == null) {
        return "";
    }
    fileTitle = fileTitle.trim();
    var posExt = fileTitle.lastIndexOf(".");
    return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : "";
};

ASC.Files.Utility.FileInAnInternalFormat = function (fileTitle) {
    var fileExt = ASC.Files.Utility.GetFileExtension(fileTitle);
    for (var i in ASC.Files.Utility.Resource.InternalFormats) {
        if (ASC.Files.Utility.Resource.InternalFormats[i] === fileExt) {
            return true;
        }
    }
    return false;
};

ASC.Files.Utility.CanImageView = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsImagePreviewed) != -1;
};

ASC.Files.Utility.CanWebView = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsWebPreviewed) != -1;
};

ASC.Files.Utility.CanWebEdit = function (fileTitle) {
    return (
        ASC.Files.Utility.CanWebEditBrowser && Teamlab.profile.isVisitor !== true
            ? jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsWebEdited) != -1
            : false
    );
};

ASC.Files.Utility.CanWebReview = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsWebReviewed) != -1;
};

ASC.Files.Utility.CanCoAuhtoring = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsCoAuthoring) != -1;
};

ASC.Files.Utility.MustConvert = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsMustConvert) != -1;
};

ASC.Files.Utility.GetConvertFormats = function (fileTitle) {
    var curExt = ASC.Files.Utility.GetFileExtension(fileTitle);
    return ASC.Files.Utility.Resource.ExtsConvertible[curExt] || [];
};

ASC.Files.Utility.FileIsArchive = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsArchive) != -1;
};

ASC.Files.Utility.FileIsVideo = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsVideo) != -1;
};

ASC.Files.Utility.FileIsAudio = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsAudio) != -1;
};

ASC.Files.Utility.FileIsImage = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsImage) != -1;
};

ASC.Files.Utility.FileIsSpreadsheet = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsSpreadsheet) != -1;
};

ASC.Files.Utility.FileIsPresentation = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsPresentation) != -1;
};

ASC.Files.Utility.FileIsDocument = function (fileTitle) {
    return jq.inArray(ASC.Files.Utility.GetFileExtension(fileTitle), ASC.Files.Utility.Resource.ExtsDocument) != -1;
};

ASC.Files.Utility.GetFileViewUrl = function (fileId, fileVersion) {
    var url = ASC.Files.Utility.Resource.FileViewUrlString.format(encodeURIComponent(fileId));
    if (fileVersion) {
        return url + "&" + ASC.Files.Utility.Resource.ParamVersion + "=" + fileVersion;
    }
    return url;
};

ASC.Files.Utility.GetFileDownloadUrl = function (fileId, fileVersion, convertToExtension) {
    var url = ASC.Files.Utility.Resource.FileDownloadUrlString.format(encodeURIComponent(fileId));
    if (fileVersion) {
        return url + "&" + ASC.Files.Utility.Resource.ParamVersion + "=" + fileVersion;
    }
    if (convertToExtension) {
        return url + "&" + ASC.Files.Utility.Resource.ParamOutType + "=" + convertToExtension;
    }
    return url;
};

ASC.Files.Utility.GetFileWebViewerUrl = function (fileId, fileVersion) {
    var url = ASC.Files.Utility.Resource.FileWebViewerUrlString.format(encodeURIComponent(fileId));
    if (fileVersion) {
        return url + "&" + ASC.Files.Utility.Resource.ParamVersion + "=" + fileVersion;
    }
    return url;
};

ASC.Files.Utility.GetFileWebViewerExternalUrl = function (fileUri, fileTitle, refererUrl) {
    return ASC.Files.Utility.Resource.FileWebViewerExternalUrlString.format(encodeURIComponent(fileUri), encodeURIComponent(fileTitle || ""), encodeURIComponent(refererUrl || ""));
};

ASC.Files.Utility.GetFileWebEditorUrl = function (fileId) {
    return ASC.Files.Utility.Resource.FileWebEditorUrlString.format(encodeURIComponent(fileId));
};

ASC.Files.Utility.GetFileWebEditorExternalUrl = function (fileUri, fileTitle, folderId) {
    return ASC.Files.Utility.Resource.FileWebEditorExternalUrlString.format(encodeURIComponent(fileUri), encodeURIComponent(fileTitle || "")) +
        ((folderId || "") ? ("&folderid=" + folderId) : "");
};