class Glass {
    constructor() {
        // fields 
        this._seconds = 0;
        this.isTarget = false; 
        this._isRelatedToTarget = false;
        this._childLeft = null; 
        this._childRight = null; 
        this._parentLeft = null;
        this._parentRight = null;        
    }

    /// <summary>
    /// Adds the specified seconds to the Glass. Any overflow is directed to the Glasses supporting it.
    /// </summary>
    /// <param name="seconds">The seconds to add to the Glass.</param>
    Add(seconds) {
        let secondsOverflow = (this._seconds + seconds) - Glass._fillSpeedSec;
        if (secondsOverflow > 0) {
            // fill seconds is more than we can handle, fill us to the limit
            this._seconds = Glass._fillSpeedSec;

            // let the caller know that the target is full
            if (this._isTarget)
                return true;

            if ((this._childLeft != null && this._childRight != null) && (this._childLeft._isRelatedToTarget || this._childRight._isRelatedToTarget)) {
                secondsOverflow /= 2;
                if (this._childLeft._isRelatedToTarget && this._childLeft.Add(secondsOverflow))
                    return true;
                if (this._childRight._isRelatedToTarget && this._childRight.Add(secondsOverflow))
                    return true;
            }
        } else {
            // fill without overflowing
            this._seconds += seconds;
        }

        return false;
    }

    /// <summary>
    /// Creates a jagged array that represents a Pyramid of Glass objects, each Glass object's Left 
    /// and Right properties are connected to the Glasses that supports it. One of the Glasses is to be
    /// marked as the target.
    /// </summary>
    /// <param name="glassRows">The number of rows to create for the Pyramid of Glass objects</param>
    /// <param name="glassRowTarget">The row number (1-based) for the Glass object to mark as the target</param>
    /// <param name="glassColTarget">The column number (1-based) for the Glass object to mark as the target</param>
    static PyramidCreate(glassRowTarget, glassColTarget) {
        // create a jagged array that will represent our Pyramid of Glass objects
        let result = new Array(glassRowTarget);
        let glassTarget = null;
        for (let y = 0; y < glassRowTarget; y++) {
            result[y] = new Array(y + 1);
            for (let x = 0; x <= y; x++) {
                let isTarget = y + 1 == glassRowTarget && x + 1 == glassColTarget;
                result[y][x] = new Glass();
                result[y][x]._isTarget = isTarget;
                if (isTarget)
                    glassTarget = result[y][x];
            }
        }

        // each Glass stands on a left and right Glass, and have a left and a right parent Glass
        // (if not last/first row) - anyhow, setup this connection
        for (let y = 0; y < result.length - 1; y++) {
            for (let x = 0; x < result[y].length; x++) {
                result[y][x]._childLeft = result[y + 1][x];
                result[y + 1][x]._parentRight = result[y][x];
                result[y][x]._childRight = result[y + 1][x + 1];
                result[y + 1][x + 1]._parentLeft = result[y][x];
            }
        }

        // starting with the taget, we visit each parent to set _isRelatedToTarget on the Glasses that need processing
        let tagItem = function(g) {
            g._isRelatedToTarget = true;
            if ((g._parentLeft != null) && (!g._parentLeft._isRelatedToTarget))
                tagItem(g._parentLeft);
            if ((g._parentRight != null) && (!g._parentRight._isRelatedToTarget))
                tagItem(g._parentRight);
            return false;
        }

        tagItem(glassTarget);

        // return the top glass
        return result;
    }

    /// <summary>
    /// Calculates the maximum capacity of a Pyramid of Glass objects
    /// </summary>
    /// <param name="glassRows">The number of rows in the Pyramid</param>
    static PyramidMaxCapacity(glassRows) {
        //2=30,3=70,4=150,5=310,6=630
        let result = 0;
        for (let y = 0; y < glassRows; y++) {
            result = result * 2 + Glass._fillSpeedSec;
        }
        return result;
    }

    /// <summary>
    /// Resets the seconds property of the Pyramid of Glass objects.
    /// </summary>
    /// <param name="glassPyramid">The Pyramid of Glass objects to reset</param>
    static PyramidReset(glassPyramid) {
        for (let y = 0; y < glassPyramid.length; y++)
            for (let x = 0; x < glassPyramid[y].length; x++)
                glassPyramid[y][x]._seconds = 0;
    }

    /// <summary>
    /// Renders a Pyramid of Glass objects to the Console. The target Glass will be highlighted in green, and
    /// the related/analyzed Glass objects are highlighted in yellow.
    /// </summary>
    /// <param name="glassPyramid">The Pyramid of Glass objects to render</param>
    /// <param name="glassRowTarget">The row number (1-based) for the Glass object to highlight</param>
    /// <param name="glassColTarget">The column number (1-based) for the Glass object to highlight</param>
    static PyramidRender(glassPyramid, glassRowTarget, glassColTarget) {
        let r = "";
        for (let y = 0; y < glassPyramid.length; y++) {
            r += new Array((glassPyramid[glassPyramid.length - 1].length - y - 1) * 6 / 2).join(' ');
            for (let x = 0; x < glassPyramid[y].length; x++) {
                r += ("0000" + Math.round(glassPyramid[y][x]._seconds*100,0)).slice(-4) + " ";
            }
            r += "\n";
        }
        r += "\n";
        console.log(r);
    }
}

// time to fill a single glass 
Glass._fillSpeedSec = 10


module.exports = Glass;