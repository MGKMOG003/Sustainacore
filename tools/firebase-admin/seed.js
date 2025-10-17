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
    { email: 'admin1@sustainacore.demo',      password: 'Password123!', displayName: 'Admin One',      role: 'Admin' },
    { email: 'admin2@sustainacore.demo',      password: 'Password123!', displayName: 'Admin Two',      role: 'Admin' },
    { email: 'pm1@sustainacore.demo',         password: 'Password123!', displayName: 'PM One',         role: 'ProjectManager' },
    { email: 'pm2@sustainacore.demo',         password: 'Password123!', displayName: 'PM Two',         role: 'ProjectManager' },
    { email: 'contractor1@sustainacore.demo', password: 'Password123!', displayName: 'Contractor One', role: 'Contractor' },
    { email: 'contractor2@sustainacore.demo', password: 'Password123!', displayName: 'Contractor Two', role: 'Contractor' },
    { email: 'client1@sustainacore.demo',     password: 'Password123!', displayName: 'Client One',     role: 'Client' },
    { email: 'client2@sustainacore.demo',     password: 'Password123!', displayName: 'Client Two',     role: 'Client' }
  ];
  for (const u of users) {
    await ensureUser(u.email, u.password, u.displayName, u.role);
    console.log(`Seeded ${u.email}`);
  }
  console.log('All demo users created or updated.');
}

main().catch(err => console.error(err));
