namespace QualityBot.Util
{
    using QualityBot.Util.Enums;

    // TODO: move to resource files
    public static class Javascript
    {
        public const string JQuery =
            @"var head=document.getElementsByTagName(""head"")[0]||document.documentElement;var script=document.createElement(""script"");script.src=""https://ajax.googleapis.com/ajax/libs/jquery/1.8.1/jquery.min.js"";var done=false;script.onload=script.onreadystatechange=function(){if(!done&&(!this.readyState||this.readyState===""loaded""||this.readyState===""complete"")){done=true;script.onload=script.onreadystatechange=null;window.$QBjQuery=jQuery.noConflict(true);if(head&&script.parentNode){head.removeChild(script)}callback('done');}};head.insertBefore(script,head.firstChild);";

        public const string GetText =
            @"$QBjQuery.fn.justtext=function(){return $QBjQuery(this).clone().children().remove().end().text()};";

        public const string GetCss =
            @"$QBjQuery.fn.getStyleObject=function(){var a=this.get(0),b,c={},d,e,f,g,h,i;d=function(x,y){return y.toUpperCase()};try{if(window.getComputedStyle){b=window.getComputedStyle(a,null);for(e=0,f=b.length;e<f;e+=1){g=b[e];h=g.replace(/\-([a-z])/g,d);i=b.getPropertyValue(g);c[h]=i}return c}if(b=a.currentStyle){for(g in b){c[g]=b[g]}return c}if(b=a.style){for(g in b){if(typeof b[g]!=""function""){c[g]=b[g]}}return c}return c}catch(j){if(window.getComputedStyle){b=window.getComputedStyle(a,null);for(e=0,f=b.length;e<f;e+=1){g=b[e];h=g.replace(/\-([a-z])/g,d);i=b.getPropertyValue(g);c[h]=i}return c}if(b=a.currentStyle){for(g in b){try{c[g]=b[g]}catch(j){c[g]=""""}}return c}if(b=a.style){for(g in b){if(typeof b[g]!=""function""){try{c[g]=b[g]}catch(j){c[g]=""""}}}return c}return c}}";

        public const string GetRects =
            @"{var r=new Array;$QBjQuery(%INCLUDE%).filter(':visible').filter(function(){return!($QBjQuery(this).css('visibility')=='hidden'||$QBjQuery(this).css('display')=='none'||!($QBjQuery(this).outerWidth()>0&&$QBjQuery(this).outerHeight()>0))}).not(%EXCLUDE%).each(function(a,b){$QBjQuery(this).attr('diffengineindexer',a);r.push({x:$QBjQuery(this).offset().left,y:$QBjQuery(this).offset().top,width:$QBjQuery(this).outerWidth(),height:$QBjQuery(this).outerHeight(),text:$QBjQuery(this).justtext(),css:$QBjQuery(this).getStyleObject()})});return JSON.stringify(r);}";

        public const string Viewport =
            @"{var a;var b;if(typeof window.innerWidth!=""undefined""){a=window.innerWidth,b=window.innerHeight}else if(typeof document.documentElement!=""undefined""&&typeof document.documentElement.clientWidth!=""undefined""&&document.documentElement.clientWidth!=0){a=document.documentElement.clientWidth,b=document.documentElement.clientHeight}else{a=document.getElementsByTagName(""body"")[0].clientWidth,b=document.getElementsByTagName(""body"")[0].clientHeight}return[a,b]}";

        public const string Info =
            @"{var BrowserDetect={init:function(){this.browser=this.searchString(this.dataBrowser)||""An unknown browser"";this.version=this.searchVersion(navigator.userAgent)||this.searchVersion(navigator.appVersion)||""anunknown version"";this.OS=this.searchString(this.dataOS)||""an unknown OS""},searchString:function(a){for(var b=0;b<a.length;b++){var c=a[b].string;var d=a[b].prop;this.versionSearchString=a[b].versionSearch||a[b].identity;if(c){if(c.indexOf(a[b].subString)!=-1)return a[b].identity}else if(d)return a[b].identity}},searchVersion:function(a){var b=a.indexOf(this.versionSearchString);if(b==-1)return;return parseFloat(a.substring(b+this.versionSearchString.length+1))},dataBrowser:[{string:navigator.userAgent,subString:""Chrome"",identity:""Chrome""},{string:navigator.userAgent,subString:""OmniWeb"",versionSearch:""OmniWeb/"",identity:""OmniWeb""},{string:navigator.vendor,subString:""Apple"",identity:""Safari"",versionSearch:""Version""},{prop:window.opera,identity:""Opera"",versionSearch:""Version""},{string:navigator.vendor,subString:""iCab"",identity:""iCab""},{string:navigator.vendor,subString:""KDE"",identity:""Konqueror""},{string:navigator.userAgent,subString:""Firefox"",identity:""Firefox""},{string:navigator.vendor,subString:""Camino"",identity:""Camino""},{string:navigator.userAgent,subString:""Netscape"",identity:""Netscape""},{string:navigator.userAgent,subString:""MSIE"",identity:""Explorer"",versionSearch:""MSIE""},{string:navigator.userAgent,subString:""Gecko"",identity:""Mozilla"",versionSearch:""rv""},{string:navigator.userAgent,subString:""Mozilla"",identity:""Netscape"",versionSearch:""Mozilla""}],dataOS:[{string:navigator.platform,subString:""Win"",identity:""Windows""},{string:navigator.platform,subString:""Mac"",identity:""Mac""},{string:navigator.userAgent,subString:""iPhone"",identity:""iPhone/iPod""},{string:navigator.platform,subString:""Linux"",identity:""Linux""}]};BrowserDetect.init();return [BrowserDetect.browser,BrowserDetect.version,BrowserDetect.OS];}";

        public const string Resources =
            @"{var res = Array();$QBjQuery('[src]').each(function(){res.push($QBjQuery(this).get(0).src);});$QBjQuery('[href]:not(a,iframe)').each(function(){res.push(this.href);});return res;}";

        public static string GetLoginScript(string username, ImsLevel level)
        {
            var ims = new Ims(level);
            string userId;

            var att = ims.GetAtt(username, out userId);

            var script = string.Format(@"{{document.cookie = ""{0}="" + escape(""{1}"") + ", "ATT", att) +
                string.Format(@"("";path={0}"") + ("";domain={1}""); }}", "/", string.Format(".{0}.com", ims.EnvironmentLevelDictionary[level.ToString()]));

            return script;
        }
    }
}