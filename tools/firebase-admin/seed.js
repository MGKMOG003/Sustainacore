// tools/firebase-admin/seed.js
const admin = require('firebase-admin');
require('dotenv').config({ path: '.env.local' });

admin.initializeApp({
  credential: admin.credential.applicationDefault(),
  projectId: process.env.FIREBASE_PROJECT_ID
});

const auth = admin.auth();
const db = admin.firestore();

async function ensureUser(email, password, displayName, role) {
  let user;
  try {
    user = await auth.getUserByEmail(email);
    console.log(`Found: ${email}`);
  } catch {
    user = await auth.createUser({ email, password, displayName, emailVerified: true });
    console.log(`Created: ${email}`);
  }
  await auth.setCustomUserClaims(user.uid, { role });
  await db.collection('users').doc(user.uid).set({
    email,
    displayName,
    role,
    createdAt: admin.firestore.FieldValue.serverTimestamp()
  }, { merge: true });
  return user.uid;
}

async function main() {
  const demo = [
    { email: 'admin1@sustainacore.local',  pw: 'Passw0rd!', name: 'Admin One',  role: 'Admin'  },
    { email: 'admin2@sustainacore.local',  pw: 'Passw0rd!', name: 'Admin Two',  role: 'Admin'  },
    { email: 'client1@sustainacore.local', pw: 'Passw0rd!', name: 'Client One', role: 'Client' },
    { email: 'client2@sustainacore.local', pw: 'Passw0rd!', name: 'Client Two', role: 'Client' },
  ];
  for (const u of demo) await ensureUser(u.email, u.pw, u.name, u.role);
  console.log('Seeding complete.');
}
main().catch(err => { console.error(err); process.exit(1); });
