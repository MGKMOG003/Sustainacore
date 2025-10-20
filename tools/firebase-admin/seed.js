const admin = require('firebase-admin');
require('dotenv').config({ path: '.env.local' });

admin.initializeApp({
  credential: admin.credential.applicationDefault(),
  projectId: process.env.FIREBASE_PROJECT_ID
});
const auth = admin.auth();

async function ensureUser(email, password, displayName, role) {
  let user;
  try {
    user = await auth.getUserByEmail(email);
  } catch {
    user = await auth.createUser({
      email,
      password,
      displayName,
      emailVerified: true
    });
  }
  await auth.setCustomUserClaims(user.uid, { role });
}

async function main() {
  const users = [

    
  ];
  for (const u of users) {
    await ensureUser(u.email, u.password, u.displayName, u.role);
    console.log(`Seeded ${u.email}`);
  }
  console.log('All demo users created or updated.');
}

main().catch(err => console.error(err));
