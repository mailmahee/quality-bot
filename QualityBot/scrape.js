var page = require('webpage').create();
var fs = require('fs');

var address, width, height, include, exclude, script, callback;

if (phantom.args.length != 7) {
    console.log('Usage: scrape.js URL WIDTH HEIGHT INCLUDE EXCLUDE SCRIPT CALLBACK');
    phantom.exit();
} else {
	address = phantom.args[0];
	width = phantom.args[1];
	height = phantom.args[2];
	include = phantom.args[3];
	exclude = phantom.args[4];
	script = phantom.args[5];
	callback = phantom.args[6];
    console.log("url: "+ address);
    console.log("width: " + width);
    console.log("height: " + height);
    console.log("include: " + include);
    console.log("exclude: " + exclude);
    console.log("script: " + script);
    console.log("callback: " + callback);
}

page.viewportSize = { width: width, height: height };
page.open(address, function (status) { 
	if (status !== 'success') {
		console.log('Unable to load the address!');
		phantom.exit();
	} else {
	    page.includeJs('https://ajax.googleapis.com/ajax/libs/jquery/1.8.1/jquery.min.js', function () {
            // jQuery no conflict
	        page.evaluate(function () { window.$QBjQuery = jQuery.noConflict(true); });
	        
            // Add needed functions
	        addJqueryFunctions();

			// run user script
			page.injectJs(script);
				
			// get viewport size
			var size = getViewportSize();
				
			// get page resources
			var resources = getResources();
				
			// get screenshot
			var screenshot = page.renderBase64();
				
			// get element information
			var elements = getElementInformation(include, exclude);
				
			// get page source
			var html = page.content;
				
			fs.write(callback, JSON.stringify(size), "a");
			fs.write(callback, "\r\n", "a");
			fs.write(callback, JSON.stringify(resources), "a");
			fs.write(callback, "\r\n", "a");
			fs.write(callback, screenshot, "a");
			fs.write(callback, "\r\n", "a");
			fs.write(callback, elements, "a");
			fs.write(callback, "\r\n", "a");
			fs.write(callback, JSON.stringify(page.cookies), "a");
			fs.write(callback, "\r\n", "a");
			fs.write(callback, html, "a");
				
			phantom.exit();
		}, script, include, exclude);
	}
});

function getElementInformation(i, e) {
    var elements = page.evaluate(function(i, e) {
        var r = new Array;
        $QBjQuery(i).filter(':visible').filter(function() {
            return !($QBjQuery(this).css('visibility') == 'hidden'
                || $QBjQuery(this).css('display') == 'none'
                || !($QBjQuery(this).outerWidth() > 0 && $QBjQuery(this).outerHeight() > 0));
        }).not(e).each(function(a, b) {
            $QBjQuery(this).attr('diffengineindexer', a);
            r.push({
                x: $QBjQuery(this).offset().left,
                y: $QBjQuery(this).offset().top,
                width: $QBjQuery(this).outerWidth(),
                height: $QBjQuery(this).outerHeight(),
                text: $QBjQuery(this).justtext(),
                css: $QBjQuery(this).getStyleObject()
            });
        });
        return JSON.stringify(r);
    }, i, e);
	
	return elements;
}

function getResources() {

    var resources = page.evaluate(function () {
        var res = Array();
        $QBjQuery('[src]').each(function() {
            res.push($QBjQuery(this).get(0).src);
        });
        $QBjQuery('[href]:not(a,iframe)').each(function() {
            res.push(this.href);
        });
        return res;
    });
	
	return resources;
}

function addJqueryFunctions() {
    page.evaluate(function () {
        $QBjQuery.fn.justtext = function () {
            return $QBjQuery(this).clone().children().remove().end().text();
		};

        $QBjQuery.fn.getStyleObject = function() {
            var a = this.get(0), b, c = { }, d, e, f, g, h, i;
            d = function(x, y) { return y.toUpperCase(); };
            try {
                if (window.getComputedStyle) {
                    b = window.getComputedStyle(a, null);
                    for (e = 0, f = b.length; e < f; e += 1) {
                        g = b[e];
                        h = g.replace(/\-([a-z])/g, d);
                        i = b.getPropertyValue(g);
                        c[h] = i;
                    }
                    return c;
                }
                if (b = a.currentStyle) {
                    for (g in b) {
                        c[g] = b[g];
                    }
                    return c;
                }
                if (b = a.style) {
                    for (g in b) {
                        if (typeof b[g] != 'function') {
                            c[g] = b[g];
                        }
                    }
                    return c;
                }
                return c;
            } catch(j) {
                if (window.getComputedStyle) {
                    b = window.getComputedStyle(a, null);
                    for (e = 0, f = b.length; e < f; e += 1) {
                        g = b[e];
                        h = g.replace(/\-([a-z])/g, d);
                        i = b.getPropertyValue(g);
                        c[h] = i;
                    }
                    return c;
                }
                if (b = a.currentStyle) {
                    for (g in b) {
                        try {
                            c[g] = b[g];
                        } catch(j) {
                            c[g] = '';
                        }
                    }
                    return c;
                }
                if (b = a.style) {
                    for (g in b) {
                        if (typeof b[g] != 'function') {
                            try {
                                c[g] = b[g];
                            } catch(j) {
                                c[g] = '';
                            }
                        }
                    }
                    return c;
                }
                return c;
            }
        };
	});
}

function getViewportSize() {
	var size = page.evaluate(function() { 
		var a, b;
		if (typeof window.innerWidth != "undefined") {
			a = window.innerWidth;
			b = window.innerHeight;
		} else if (typeof document.documentElement != "undefined" 
				   && typeof document.documentElement.clientWidth != "undefined" 
				   && document.documentElement.clientWidth != 0) {
			a = document.documentElement.clientWidth;
			b = document.documentElement.clientHeight;
		} else {
			a = document.getElementsByTagName("body")[0].clientWidth;
			b = document.getElementsByTagName("body")[0].clientHeight;
		}
		return [a,b];
	});
	
	return size;
}