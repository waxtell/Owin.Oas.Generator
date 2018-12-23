#!/usr/bin/env node
"use strict";
var c = require('child_process');

// Initialize
process.title = 'Owin.Oas.Generator';
console.log("Owin Oas Generator CLI");
var args = process.argv.splice(2, process.argv.length - 2).map(function (a) { return a.indexOf(" ") === -1 ? a : '"' + a + '"' }).join(" ");

var cmd = '"' + __dirname + '/binaries/tools/Owin.Oas.Generator.exe" ' + args;
var code = c.execSync(cmd, { stdio: [0, 1, 2] });