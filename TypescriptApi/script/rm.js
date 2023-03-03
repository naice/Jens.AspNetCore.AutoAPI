const fs = require("fs");
const args = process.argv;

if (args.length < 2) {
    console.error("PATH REQUIRED");
    process.exit(1);
}

fs.rmSync(args[2], { recursive: true, force: true });