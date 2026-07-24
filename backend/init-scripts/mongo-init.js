// MongoDB initialization script — runs once on first container start.
// Creates per-service databases and their collections with indexes.

const adminDb = db.getSiblingDB('admin');
adminDb.auth('ehr_admin', 'ehr_mongo_password');

// ── Clinical documents database ───────────────────────────────────────────────
const clinicalDb = db.getSiblingDB('ehr_clinical');

clinicalDb.createCollection('clinical-notes');
clinicalDb['clinical-notes'].createIndex({ entityId: 1 }, { unique: false });
clinicalDb['clinical-notes'].createIndex({ 'tenantId': 1 });
clinicalDb['clinical-notes'].createIndex({ createdAt: -1 });
clinicalDb['clinical-notes'].createIndex({ deletedAt: 1 }, { sparse: true });

clinicalDb.createCollection('progress-notes');
clinicalDb['progress-notes'].createIndex({ entityId: 1 });
clinicalDb['progress-notes'].createIndex({ createdAt: -1 });

// ── Patient document store ────────────────────────────────────────────────────
const patientDb = db.getSiblingDB('ehr_patient');

patientDb.createCollection('scanned-document-metadatas');
patientDb['scanned-document-metadatas'].createIndex({ entityId: 1 });
patientDb['scanned-document-metadatas'].createIndex({ createdAt: -1 });

// ── Audit logs database (high-volume, immutable) ──────────────────────────────
const auditDb = db.getSiblingDB('ehr_audit');

auditDb.createCollection('audit-logs');
auditDb['audit-logs'].createIndex({ userId: 1, timestamp: -1 });
auditDb['audit-logs'].createIndex({ resourceType: 1, resourceId: 1 });
auditDb['audit-logs'].createIndex({ timestamp: -1 });
auditDb['audit-logs'].createIndex({ correlationId: 1 }, { sparse: true });
// TTL: retain audit logs for 7 years (HIPAA requirement)
auditDb['audit-logs'].createIndex(
  { timestamp: 1 },
  { expireAfterSeconds: 7 * 365 * 24 * 3600, name: 'hipaa_retention_ttl' }
);

print('EHR MongoDB initialization complete.');
