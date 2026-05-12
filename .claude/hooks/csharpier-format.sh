#!/usr/bin/env bash
# PostToolUse(Edit|Write) 후 자동 실행 — .cs 파일만 CSharpier로 정렬

FILE=$(node -e "try{process.stdout.write(JSON.parse(process.env.CLAUDE_TOOL_INPUT||'{}').file_path||'')}catch(e){}" 2>/dev/null)

[[ "$FILE" != *.cs ]] && exit 0

dotnet csharpier format "$FILE"
echo "[csharpier-format] 정렬 완료: $FILE"
