import 'dotenv/config';
import fs from 'node:fs';
import admin from 'firebase-admin';

if (process.env.ALLOW_SEEDING !== 'true') {
  console.error('Seeding is disabled. Set ALLOW_SEEDING=true to proceed.');
  process.exit(1);
}

const keyPath = process.env.GOOGLE_APPLICATION_CREDENTIALS;
if (!keyPath || !fs.existsSync(keyPath)) {
  console.error('Missing GOOGLE_APPLICATION_CREDENTIALS or serviceAccount.json not found.');
  process.exit(1);
}

admin.initializeApp({
  credential: admin.credential.cert(JSON.parse(fs.readFileSync(keyPath, 'utf8'))),
  projectId: process.env.FIREBASE_PROJECT_ID
});

const auth = admin.auth();
const db = admin.firestore();

// Define your seed users here (edit as needed).
// Passwords must meet your Auth password policy.
const users = [
  { email: 'admin1@sustainacore.test',      password: 'Password123!', displayName: 'Admin One',       role: 'Admin' },
  { email: 'pm1@sustainacore.test',         password: 'Password123!', displayName: 'PM One',          role: 'ProjectManager' },
  { email: 'contractor1@sustainacore.test', password: 'Password123!', displayName: 'Contractor One',  role: 'Contractor' },
  { email: 'client1@sustainacore.test',     password: 'Password123!', displayName: 'Client One',      role: 'Client' }
];

// Upsert an Auth user, set claims, and ensure a Firestore profile doc.
async function upsertUser(u) {
  let record;
  try {
    record = await auth.getUserByEmail(u.email);
    // Update password/displayName if you want to keep them in sync on reruns:
    await auth.updateUser(record.uid, {
      displayName: u.displayName,
      password: u.password
    });
  } catch {
    record = await auth.createUser({
      email: u.email,
      password: u.password,
      displayName: u.displayName,
      emailVerified: true
    });
  }

  // Set/refresh custom claims (role).
  await auth.setCustomUserClaims(record.uid, { role: u.role });

  // Create/update Firestore profile
  const userDoc = db.collection('users').doc(record.uid);
  await userDoc.set({
    email: u.email,
    displayName: u.displayName,
    role: u.role,
    createdAt: admin.firestore.FieldValue.serverTimestamp(),
    updatedAt: admin.firestore.FieldValue.serverTimestamp()
  }, { merge: true });

  console.log(`✓ ${u.email} → ${u.role}`);
}

(async () => {
  try {
    for (const u of users) {
      try {
        await upsertUser(u);
      } catch (err) {
        console.error(`✗ ${u.email}`, err?.message || err);
      }
    }
    console.log('Seeding complete.');
    process.exit(0);
  } catch (e) {
    console.error('Seeding failed:', e?.message || e);
    process.exit(1);
  }
})();
