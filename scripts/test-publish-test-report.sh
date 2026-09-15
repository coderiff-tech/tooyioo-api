#!/usr/bin/env bash

set -euo pipefail

script_directory="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
temporary_directory="$(mktemp -d)"
trap 'rm -rf "$temporary_directory"' EXIT

report_path="${temporary_directory}/merged-report.html"
pages_directory="${temporary_directory}/pages"
printf '<html><body>TUnit fixture</body></html>' > "$report_path"

run_publisher() {
  DRY_RUN=true TEST_REPORT_PATH="$report_path" PAGES_WORK_DIR="$pages_directory" TEST_RESULT="$1" RUN_ID="$2" RUN_NUMBER="$3" SHA="1234567890abcdef" SERVER_URL="https://github.example" REPOSITORY="tooyioo/api" \
    bash "${script_directory}/publish-test-report.sh"
}

run_publisher success 100 1
run_publisher failure 101 2

test -f "${pages_directory}/runs/100/index.html"
test -f "${pages_directory}/runs/101/index.html"
test -f "${pages_directory}/latest/index.html"
test "$(jq 'length' "${pages_directory}/history.json")" = "2"
test "$(jq -r '.[0].status' "${pages_directory}/history.json")" = "PASSED"
test "$(jq -r '.[1].status' "${pages_directory}/history.json")" = "FAILED"
grep -q 'runs/101/' "${pages_directory}/index.html"
grep -q 'class="failed">FAILED' "${pages_directory}/index.html"

jq -n '[range(1; 101) | { run: tostring, number: tostring, date: "fixture", status: "PASSED", sha: "fixture", url: "https://github.example" }]' > "${pages_directory}/history.json"
run_publisher success 101 101

test "$(jq 'length' "${pages_directory}/history.json")" = "100"
test "$(jq -r '.[0].run' "${pages_directory}/history.json")" = "2"
test "$(jq -r '.[99].run' "${pages_directory}/history.json")" = "101"

echo "publish-test-report dry-run checks passed"
