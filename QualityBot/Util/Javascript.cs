namespace QualityBot.Util
{
    public static class Javascript
    {
        public const string JQuery =
            @"{var b=document.getElementsByTagName('body')[0]; if(typeof jQuery=='undefined'){var script=document"
            + @".createElement('script'); script.src='http://code.jquery.com/jquery-latest.min.js';var head=document"
            + @".getElementsByTagName('head')[0],done=false;script.onload=script.onreadystatechange=function(){if(!"
            + @"done&&(!this.readyState||this.readyState=='loaded'||this.readyState=='complete')){done=true;script."
            + @"onload=script.onreadystatechange=null;head.removeChild(script);}};head.appendChild(script);}}";

        public const string GetText =
            @"jQuery.fn.justtext=function(){return $(this).clone().children().remove().end().text()};";

        public const string GetCss =
            @"$.fn.getStyleObject=function(){var a=this.get(0);var b;var c={};if(window.getComputedStyle){var d=func"
            + @"tion(a,b){return b.toUpperCase()};b=window.getComputedStyle(a,null);for(var e=0,f=b.length;e<f;e++){"
            + @"var g=b[e];var h=g.replace(/\-([a-z])/g,d);var i=b.getPropertyValue(g);c[h]=i}return c}if(b=a.curren"
            + @"tStyle){for(var g in b){c[g]=b[g]}return c}if(b=a.style){for(var g in b){if(typeof b[g]!=""function"""
            + @"){c[g]=b[g]}}return c}return c};";

        public const string GetRects =
            @"var r = new Array();$(%INCLUDE%).filter(':visible').filter(function() { return !($(this).css('visibility')"
            + @" == 'hidden' || $(this).css('display') == 'none' || !($(this).outerWidth() > 0 && $(this).outerHeight()"
            + @" > 0));}).not(%EXCLUDE%).each(function (a, b) {$(this).attr('diffengineindexer', a);r.push({x:$(this).offset().left,y:"
            + @"$(this).offset().top,width:$(this).outerWidth(),height:$(this).outerHeight(),text:$(this).justtext(),cs"
            + @"s:$(this).getStyleObject()});});return r;";

        public const string Viewport =
            @"{var a;var b;if(typeof window.innerWidth!=""undefined""){a=window.innerWidth,b=window.innerHeight}else if"
            + @"(typeof document.documentElement!=""undefined""&&typeof document.documentElement.clientWidth!=""undefin"
            + @"ed""&&document.documentElement.clientWidth!=0){a=document.documentElement.clientWidth,b=document.docume"
            + @"ntElement.clientHeight}else{a=document.getElementsByTagName(""body"")[0].clientWidth,b=document.getElem"
            + @"entsByTagName(""body"")[0].clientHeight}return[a,b]}";

        public const string Info =
            @"{var BrowserDetect={init:function(){this.browser=this.searchString(this.dataBrowser)||""An unknown browser"
            + @""";this.version=this.searchVersion(navigator.userAgent)||this.searchVersion(navigator.appVersion)||""an"
            + @" unknown version"";this.OS=this.searchString(this.dataOS)||""an unknown OS""},searchString:function(a){"
            + @"for(var b=0;b<a.length;b++){var c=a[b].string;var d=a[b].prop;this.versionSearchString=a[b].versionSear"
            + @"ch||a[b].identity;if(c){if(c.indexOf(a[b].subString)!=-1)return a[b].identity}else if(d)return a[b].ide"
            + @"ntity}},searchVersion:function(a){var b=a.indexOf(this.versionSearchString);if(b==-1)return;return pars"
            + @"eFloat(a.substring(b+this.versionSearchString.length+1))},dataBrowser:[{string:navigator.userAgent,subS"
            + @"tring:""Chrome"",identity:""Chrome""},{string:navigator.userAgent,subString:""OmniWeb"",versionSearch:"""
            + @"OmniWeb/"",identity:""OmniWeb""},{string:navigator.vendor,subString:""Apple"",identity:""Safari"",versio"
            + @"nSearch:""Version""},{prop:window.opera,identity:""Opera"",versionSearch:""Version""},{string:navigator"
            + @".vendor,subString:""iCab"",identity:""iCab""},{string:navigator.vendor,subString:""KDE"",identity:""Kon"
            + @"queror""},{string:navigator.userAgent,subString:""Firefox"",identity:""Firefox""},{string:navigator.ven"
            + @"dor,subString:""Camino"",identity:""Camino""},{string:navigator.userAgent,subString:""Netscape"",identi"
            + @"ty:""Netscape""},{string:navigator.userAgent,subString:""MSIE"",identity:""Explorer"",versionSearch:""M"
            + @"SIE""},{string:navigator.userAgent,subString:""Gecko"",identity:""Mozilla"",versionSearch:""rv""},{stri"
            + @"ng:navigator.userAgent,subString:""Mozilla"",identity:""Netscape"",versionSearch:""Mozilla""}],dataOS:["
            + @"{string:navigator.platform,subString:""Win"",identity:""Windows""},{string:navigator.platform,subString:"
            + @"""Mac"",identity:""Mac""},{string:navigator.userAgent,subString:""iPhone"",identity:""iPhone/iPod""},{st"
            + @"ring:navigator.platform,subString:""Linux"",identity:""Linux""}]};BrowserDetect.init();return [BrowserDe"
            + @"tect.browser,BrowserDetect.version,BrowserDetect.OS];}";

        public const string Resources =
            @"{var res = Array();$('[src]').each(function(){res.push($(this).get(0).src);});$('[href]:not(a,iframe)').ea"
            + @"ch(function(){res.push(this.href);});return res;}";
    }
}