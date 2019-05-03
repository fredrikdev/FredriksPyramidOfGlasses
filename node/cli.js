#!/usr/bin/env node

/*
    Start with "npm run start"
*/

const Glass = require('./glass.js')
const reader = require("readline-sync");

class Program {
    static main() {
        // init
        console.log("Fredrik's Pyramid of Glasses v1.0. Copyright (c) by Fredrik Johansson 2019 (fredrik@johanssonrobotics.com)\n");

        // index (1-based) of the row and column of the glass that we're targeting
        // glassRow also equals to the number of rows in our glass pyramid, first row has 1 glass, second row has 2, and so on
        let glassRow = 35, glassCol = 10;

        // prompt for parameters (just ENTER will exit)
        var r = { v: 0 }
        if (!this.prompt("Enter the number of rows (2 - 50) for the Pyramid, then press ENTER: ", r, 2, 50))
            return;
        glassRow = r.v;

        if (!this.prompt("Enter the index of the glass on the last row to target (1 - " + glassRow + "), then press ENTER: ", r, 1, glassRow))
            return;
        glassCol = r.v;

        // compute the answer by testing different solutions on glassPyramid in the range secondsMin-secondsMax
        console.log("Computing the number of seconds it takes to fill glass " + glassCol + " on the lowest row, in a glass pyramid with " + glassRow + " rows.");
        let debugTickStartMain = new Date();
        let secondsMin = 0, secondsMax = Glass.PyramidMaxCapacity(glassRow);
        let secondsTry = secondsMax;
        let glassPyramid = Glass.PyramidCreate(glassRow, glassCol);
        let glassTop = glassPyramid[0][0];
        while (true) {
            // create a pyramid and test to see if the seconds is to much or to little
            let l = "Answer is between " + secondsMin + " and " + secondsMax + ". Testing " + secondsTry + "... ";

            let debugTickStart = new Date();
            Glass.PyramidReset(glassPyramid);
            let addRes = glassTop.Add(secondsTry);
            let debugTickElapsed = new Date() - debugTickStart;

            let secondsPrev = secondsTry;
            if (addRes) {
                // overflow
                // set the next seconds to try to the mid point between secondsMin and secondsMax 
                l = "To much (took " + debugTickElapsed + " msec to compute)";
                secondsMax = secondsTry;
                secondsTry = secondsMin + (secondsMax - secondsMin) / 2;
            } else {
                // set the next seconds to try to the mid point between secondsMin and secondsMax 
                l = "To little (took " + debugTickElapsed + " msec to compute)";
                secondsMin = secondsTry;
                secondsTry = secondsMin + (secondsMax - secondsMin) / 2;
            }

            console.log(l);

            if (this.round3(secondsTry) == this.round3(secondsPrev)) {
                // solution found (down to 3 decimals)
                let debugTickElapsedMain = new Date() - debugTickStartMain;

                console.log();
                Glass.PyramidRender(glassPyramid, glassRow, glassCol);
                console.log("Finished! On a glass pyramid with " + glassRow + " rows, the glass on row " + glassRow + ", at column " + glassCol + " is entirely filled after " + this.round3(secondsTry) + " seconds (took " + debugTickElapsedMain + " msec to compute the solution).");
                console.log("Press ENTER to exit...");
                break;
            }
        }
    }

    // prompts for user to input an integer in the given range (inclusive). a blank input will return false.
    static prompt(prompt, result, min, max) {
        while (true) {
            let l = reader.question(prompt);
            if (l.Length == 0) {
                result.v = 0;
                return false;
            }
            result.v = parseInt(l, 10)
            if (result.v >= min && result.v <= max)
                return true;
        }
    }

    static round3(v) {
        return Math.round(v*1000)/1000;
    }
}

Program.main();