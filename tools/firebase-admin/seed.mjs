import 'dotenv/config';
import admin from 'firebase-admin';
import fs from 'node:fs';

const keyPath = process.env.GOOGLE_APPLICATION_CREDENTIALS;
if (!fs.existsSync(keyPath)) { console.error('Missing serviceAccount.json'); process.exit(1); }

admin.initializeApp({
  credential: admin.credential.cert(JSON.parse(fs.readFileSync(keyPath, 'utf8'))),
  projectId: process.env.FIREBASE_PROJECT_ID
});

const users = [
  { email: 'admin1@sustainacore.test',          password: 'Password123!', displayName: 'Admin One',          role: 'Admin' },
  { email: 'admin2@sustainacore.test',          password: 'Password123!', displayName: 'Admin Two',          role: 'Admin' },
  { email: 'pm1@sustainacore.test',             password: 'Password123!', displayName: 'PM One',             role: 'ProjectManager' },
  { email: 'pm2@sustainacore.test',             password: 'Password123!', displayName: 'PM Two',             role: 'ProjectManager' },
  { email: 'contractor1@sustainacore.test',     password: 'Password123!', displayName: 'Contractor One',     role: 'Contractor' },
  { email: 'contractor2@sustainacore.test',     password: 'Password123!', displayName: 'Contractor Two',     role: 'Contractor' },
  { email: 'client1@sustainacore.test',         password: 'Password123!', displayName: 'Client One',         role: 'Client' },
  { email: 'client2@sustainacore.test',         password: 'Password123!', displayName: 'Client Two',         role: 'Client' }
];

async function upsertUser(u) {
  try {
    let record;
    try {
      record = await admin.auth().getUserByEmail(u.email);
    } catch {
      record = await admin.auth().createUser({ email: u.email, password: u.password, displayName: u.displayName, emailVerified: true });
    }
    await admin.auth().setCustomUserClaims(record.uid, { role: u.role });
    console.log(`✓ ${u.email} → ${u.role}`);
  } catch (e) {
    console.error(`✗ ${u.email}`, e.message);
  }
}

for (const u of users) await upsertUser(u);
console.log('Done.');
