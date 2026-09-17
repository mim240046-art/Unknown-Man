# Build the APK without Unity on your laptop

This project now includes a GitHub Actions workflow at `.github/workflows/android-apk.yml`.

## One-time setup

1. Create a GitHub repository.
2. Upload the **contents** of this project folder to that repository (the folder containing `Assets`, `Packages`, `ProjectSettings`, etc.).
3. In GitHub, open **Settings → Secrets and variables → Actions**.
4. Add these repository secrets:
   - `UNITY_LICENSE`
   - `UNITY_EMAIL`
   - `UNITY_PASSWORD`

GameCI uses these values to activate Unity during the cloud build. A Unity Personal license requires a one-time license activation step; the license file can then be stored as the `UNITY_LICENSE` secret.

## Start the build

Open the repository's **Actions** tab → **Unknown Man - Android APK** → **Run workflow**.

When it finishes successfully, open the workflow run and download the artifact named:

`UnknownMan-Android-APK`

That artifact contains the `.apk` file.

## Important

GitHub Actions does the actual Unity compilation. Your laptop does **not** need Unity installed.

The Android package name is configured as:

`com.unknownman.horrorsurvival`

The workflow builds an APK (`androidPackage`), not an AAB.
