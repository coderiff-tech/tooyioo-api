#!/usr/bin/env bash

set -euo pipefail

require_environment_variable() {
  local variable_name="$1"
  if [[ -z "${!variable_name:-}" ]]; then
    echo "ERROR: ${variable_name} must be set."
    exit 1
  fi
}

for variable_name in TEST_RESULT RUN_ID RUN_NUMBER SHA SERVER_URL REPOSITORY; do
  require_environment_variable "$variable_name"
done

script_directory="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
template_path="${script_directory}/templates/test-report-index.html.template"
report_path="${TEST_REPORT_PATH:-}"

if [[ -z "$report_path" ]]; then
  require_environment_variable RUNNER_TEMP
  report_path="$(find "$RUNNER_TEMP" -type f -name "merged-report.html" -print -quit)"
fi

if [[ -z "$report_path" || ! -f "$report_path" ]]; then
  echo "ERROR: TUnit merged-report.html was not found."
  exit 1
fi

echo "TUnit report: ${report_path}"

if [[ ! -f "$template_path" ]]; then
  echo "ERROR: Test report template was not found at ${template_path}."
  exit 1
fi

pages_work_directory="${PAGES_WORK_DIR:-${RUNNER_TEMP:-}/pages}"
if [[ -z "$pages_work_directory" || "$pages_work_directory" == "/pages" ]]; then
  echo "ERROR: PAGES_WORK_DIR or RUNNER_TEMP must be set."
  exit 1
fi

if [[ "${DRY_RUN:-false}" == "true" ]]; then
  mkdir -p "$pages_work_directory"
else
  require_environment_variable GITHUB_TOKEN
  mkdir -p "$pages_work_directory"
  cd "$pages_work_directory"
  git init
  git remote add origin "https://x-access-token:${GITHUB_TOKEN}@github.com/${REPOSITORY}.git"
  git fetch origin gh-pages
  git checkout -B gh-pages origin/gh-pages
fi

mkdir -p "${pages_work_directory}/runs/${RUN_ID}" "${pages_work_directory}/latest"
cp "$report_path" "${pages_work_directory}/runs/${RUN_ID}/index.html"
cp "$report_path" "${pages_work_directory}/latest/index.html"

if [[ "$TEST_RESULT" == "success" ]]; then
  status="PASSED"
else
  status="FAILED"
fi

run_url="${SERVER_URL}/${REPOSITORY}/actions/runs/${RUN_ID}"
run_date="$(date -u +"%Y-%m-%d %H:%M:%S UTC")"
history_path="${pages_work_directory}/history.json"

if [[ -f "$history_path" ]]; then
  jq --arg run "$RUN_ID" --arg number "$RUN_NUMBER" --arg date "$run_date" --arg status "$status" --arg sha "${SHA:0:7}" --arg url "$run_url" \
    '. += [{ run: $run, number: $number, date: $date, status: $status, sha: $sha, url: $url }]' \
    "$history_path" > "${history_path}.tmp"
  mv "${history_path}.tmp" "$history_path"
else
  jq -n --arg run "$RUN_ID" --arg number "$RUN_NUMBER" --arg date "$run_date" --arg status "$status" --arg sha "${SHA:0:7}" --arg url "$run_url" \
    '[{ run: $run, number: $number, date: $date, status: $status, sha: $sha, url: $url }]' > "$history_path"
fi

history_rows="$(jq -r '
  reverse[] |
  "<tr>" +
  "<td>\(.number)</td>" +
  "<td>\(.date)</td>" +
  "<td class=\"\(.status | ascii_downcase)\">\(.status)</td>" +
  "<td><code>\(.sha)</code></td>" +
  "<td><a href=\"runs/\(.run)/\">TUnit report</a></td>" +
  "<td><a href=\"\(.url)\">GitHub Actions</a></td>" +
  "</tr>"
' "$history_path")"

awk -v history_rows="$history_rows" '
  /{{HISTORY_ROWS}}/ { print history_rows; next }
  { print }
' "$template_path" > "${pages_work_directory}/index.html"

if [[ "${DRY_RUN:-false}" == "true" ]]; then
  echo "Dry run completed: ${pages_work_directory}"
  exit 0
fi

cd "$pages_work_directory"
git config user.name "github-actions[bot]"
git config user.email "41898282+github-actions[bot]@users.noreply.github.com"
git add .

if git diff --cached --quiet; then
  echo "Nothing changed."
  exit 0
fi

git commit -m "Add test report for run ${RUN_NUMBER}"
git push origin gh-pages
