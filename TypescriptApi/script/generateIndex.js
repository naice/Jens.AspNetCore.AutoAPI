const { parseArgs } = require("node:util");
const fs = require("fs");
const path = require("path");

const args = process.argv;

if (args.length < 2) {
    console.error("PATH REQUIRED");
    process.exit(1);
}

readDir = args[2]

let indexContent = "";
var dir = fs.readdirSync(readDir);
dir.forEach((filename) => { indexContent += "export * from './"+path.parse(filename).name+"'\n"; });
var indexPath = path.join(readDir, "index.ts");
console.log(indexPath);
fs.writeFileSync(indexPath, indexContent);
console.log("Generated "+ indexPath);