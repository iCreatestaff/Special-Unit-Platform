# Special Unit Frontend

Angular frontend for the Special Unit management app. This folder sits beside the local .NET backend folder.

```text
sp_backend/
  backend/
  frontend/
```

## Local Setup

Run these commands from this `frontend` folder:

```powershell
npm ci
npm start
```

The frontend dev server runs on:

```text
http://localhost:4200
```

The app expects the backend API from `../backend` to be running locally on:

```text
http://localhost:5038
```

## Useful Scripts

```powershell
npm start      # Start Angular dev server
npm run build  # Build the frontend
npm test       # Run Angular tests
```

## Folder Map

```text
src/app/components   Shared dashboard and UI components
src/app/layouts      Full and blank layouts, header, sidebar
src/app/Models       TypeScript models used by services and pages
src/app/pages        Feature pages and routed modules
src/app/services     API services for backend communication
src/assets           Images, logos, styles, and static assets
```

Main feature areas live under `src/app/pages`:

```text
account
authentication
Equipment
EquipmentStock
Maintenance
Mission
profile
request-maintenance
SubEquipment
training
```

## Generated Files

These folders/files are local build or runtime artifacts and are not source:

```text
node_modules/
dist/
.angular/
.npm-cache/
*.log
```

If the frontend behaves strangely after dependency or Angular cache changes, stop the dev server, delete `.angular/`, then run `npm start` again.
