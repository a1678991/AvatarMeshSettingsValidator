#!/bin/bash

# Script to export Avatar Mesh Settings Validator as a UnityPackage
# This is for local testing - production releases use GitHub Actions

# Configuration
UNITY_VERSION="2022.3.22f1"
PROJECT_PATH="$(dirname "$(dirname "$(realpath "$0")")")"
PACKAGE_NAME="AvatarMeshSettingsValidator"

# Get version from package.json
VERSION=$(grep -o '"version": "[^"]*"' "$PROJECT_PATH/package.json" | sed 's/"version": "//;s/"//')
if [ -z "$VERSION" ]; then
    VERSION="0.1.0"
fi

OUTPUT_FILE="$PROJECT_PATH/Builds/${PACKAGE_NAME}-v${VERSION}.unitypackage"

echo "Avatar Mesh Settings Validator - Package Exporter"
echo "================================================"
echo "Project Path: $PROJECT_PATH"
echo "Version: $VERSION"
echo "Output: $OUTPUT_FILE"
echo ""

# Create output directory
mkdir -p "$PROJECT_PATH/Builds"

# Find Unity installation
if [ -z "$UNITY_PATH" ]; then
    # Try common Unity installation paths
    if [ -f "/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity" ]; then
        UNITY_PATH="/Applications/Unity/Hub/Editor/$UNITY_VERSION/Unity.app/Contents/MacOS/Unity"
    elif [ -f "C:/Program Files/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity.exe" ]; then
        UNITY_PATH="C:/Program Files/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity.exe"
    elif [ -f "$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity" ]; then
        UNITY_PATH="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"
    else
        echo "Error: Unity $UNITY_VERSION not found. Please set UNITY_PATH environment variable."
        exit 1
    fi
fi

echo "Using Unity: $UNITY_PATH"
echo ""

# Create temporary Unity project for export
TEMP_PROJECT="$PROJECT_PATH/.temp-export"
echo "Creating temporary Unity project..."
rm -rf "$TEMP_PROJECT"
mkdir -p "$TEMP_PROJECT/Assets/$PACKAGE_NAME"
mkdir -p "$TEMP_PROJECT/ProjectSettings"

# Copy files to temporary project
cp -r "$PROJECT_PATH/Editor" "$TEMP_PROJECT/Assets/$PACKAGE_NAME/"
cp "$PROJECT_PATH/README.md" "$TEMP_PROJECT/Assets/$PACKAGE_NAME/" 2>/dev/null || true
cp "$PROJECT_PATH/CHANGELOG.md" "$TEMP_PROJECT/Assets/$PACKAGE_NAME/" 2>/dev/null || true
cp "$PROJECT_PATH/LICENSE" "$TEMP_PROJECT/Assets/$PACKAGE_NAME/" 2>/dev/null || true
cp "$PROJECT_PATH/package.json" "$TEMP_PROJECT/Assets/$PACKAGE_NAME/" 2>/dev/null || true

# Copy meta files
find "$PROJECT_PATH" -name "*.meta" -not -path "*/.*" | while read metafile; do
    relative_path="${metafile#$PROJECT_PATH/}"
    target_path="$TEMP_PROJECT/Assets/$PACKAGE_NAME/$relative_path"
    mkdir -p "$(dirname "$target_path")"
    cp "$metafile" "$target_path" 2>/dev/null || true
done

# Create minimal project settings
echo "m_EditorVersion: $UNITY_VERSION" > "$TEMP_PROJECT/ProjectSettings/ProjectVersion.txt"

# Export the package
echo "Exporting package..."
"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -quit \
    -projectPath "$TEMP_PROJECT" \
    -executeMethod AvatarTools.Editor.PackageExporter.ExportFromCommandLine \
    -outputPath "$OUTPUT_FILE" \
    -logFile "$PROJECT_PATH/Builds/export.log" 2>/dev/null

# Check if export was successful
if [ -f "$OUTPUT_FILE" ]; then
    echo ""
    echo "✅ Package exported successfully!"
    echo "📦 Output: $OUTPUT_FILE"
    echo "📏 Size: $(du -h "$OUTPUT_FILE" | cut -f1)"
    
    # Clean up
    rm -rf "$TEMP_PROJECT"
    rm -f "$PROJECT_PATH/Builds/export.log"
else
    echo ""
    echo "❌ Export failed! Check the log file:"
    echo "$PROJECT_PATH/Builds/export.log"
    cat "$PROJECT_PATH/Builds/export.log" 2>/dev/null || echo "Log file not found"
    exit 1
fi