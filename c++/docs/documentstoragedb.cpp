#include "documentstoragedb.h"
#include <QSqlQuery>
#include <QSqlError>
#include <QDebug>
#include <QDateTime>
#include <QStringList>
#include <algorithm>

namespace {
QString joinClientName(const QString &first, const QString &middle, const QString &last)
{
    QStringList parts;
    for (const QString &part : {first, middle, last}) {
        const QString trimmed = part.trimmed();
        if (!trimmed.isEmpty())
            parts << trimmed;
    }
    return parts.join(" ");
}

QString normalizeName(const QString &name)
{
    return name.simplified().toLower();
}
} // namespace

DocumentStorageDB::DocumentStorageDB(QSqlDatabase* database, const QString &tableName)
    : db(database), m_tableName(tableName) {}

bool DocumentStorageDB::createTable() {
    QSqlQuery query(*db);
    QString sql = QString(
        "CREATE TABLE IF NOT EXISTS %1 ("
        "id INTEGER PRIMARY KEY AUTOINCREMENT, "
        "userId INTEGER, "
        "clientId INTEGER, "
        "title TEXT, "
        "filePath TEXT, "
        "createdAt TEXT)"
    ).arg(m_tableName);

    bool ok = query.exec(sql);
    if(!ok) qDebug() << "❌ Failed to create" << m_tableName << "table:" << query.lastError().text();
    else qDebug() << "✅ Table" << m_tableName << "ready.";
    return ok;
}

// Helper to populate Document from query
static Document documentFromQuery(const QSqlQuery &query) {
    Document doc;
    doc.id = query.value("id").toInt();
    doc.userId = query.value("userId").toInt();
    doc.clientId = query.value("clientId").toInt();
    doc.title = query.value("title").toString();
    doc.filePath = query.value("filePath").toString();
    doc.createdAt = QDateTime::fromString(query.value("createdAt").toString(), Qt::ISODate);
    return doc;
}

bool DocumentStorageDB::addDocument(const Document &doc) {
    QSqlQuery query(*db);
    query.prepare(QString("INSERT INTO %1 (userId, clientId, title, filePath, createdAt) VALUES (?, ?, ?, ?, ?)").arg(m_tableName));
    query.addBindValue(doc.userId);
    query.addBindValue(doc.clientId);
    query.addBindValue(doc.title);
    query.addBindValue(doc.filePath);
    query.addBindValue(doc.createdAt.toString(Qt::ISODate));
    if(!query.exec()) {
        qDebug() << "❌ addDocument failed:" << query.lastError().text();
        return false;
    }
    return true;
}

bool DocumentStorageDB::updateDocument(const Document &doc) {
    QSqlQuery query(*db);
    query.prepare(QString("UPDATE %1 SET userId=?, clientId=?, title=?, filePath=?, createdAt=? WHERE id=?").arg(m_tableName));
    query.addBindValue(doc.userId);
    query.addBindValue(doc.clientId);
    query.addBindValue(doc.title);
    query.addBindValue(doc.filePath);
    query.addBindValue(doc.createdAt.toString(Qt::ISODate));
    query.addBindValue(doc.id);
    if(!query.exec()) {
        qDebug() << "❌ updateDocument failed:" << query.lastError().text();
        return false;
    }

    const qint64 affected = query.numRowsAffected();
    if (affected == 0) {
        qWarning() << "⚠️ updateDocument: no document found for id" << doc.id;
        return false;
    }

    if (affected < 0)
        qWarning() << "⚠️ updateDocument: driver did not report affected rows for id" << doc.id;

    return true;
}

bool DocumentStorageDB::deleteDocument(int documentId) {
    QSqlQuery query(*db);
    query.prepare(QString("DELETE FROM %1 WHERE id=?").arg(m_tableName));
    query.addBindValue(documentId);
    if(!query.exec()) {
        qDebug() << "❌ deleteDocument failed:" << query.lastError().text();
        return false;
    }

    const qint64 affected = query.numRowsAffected();
    if (affected == 0) {
        qWarning() << "⚠️ deleteDocument: no document found for id" << documentId;
        return false;
    }

    if (affected < 0)
        qWarning() << "⚠️ deleteDocument: driver did not report affected rows for id" << documentId;

    return true;
}

QVector<Document> DocumentStorageDB::getDocuments(int currentUserId, const QString &role,
                                                  int maxDocuments, bool *truncated) {
    QVector<Document> result;
    if (truncated)
        *truncated = false;

    if (maxDocuments <= 0) {
        qWarning() << "⚠️ getDocuments called with non-positive maxDocuments" << maxDocuments
                   << "for user" << currentUserId;
        return result;
    }

    result.reserve(std::min(maxDocuments, 256));

    QSqlQuery query(*db);

    if(role == "Admin" || role == "Manager") {
        if(!query.exec(QString("SELECT * FROM %1 ORDER BY createdAt DESC").arg(m_tableName))) {
            qDebug() << "❌ getDocuments failed:" << query.lastError().text();
            return result;
        }
    } else {
        query.prepare(QString("SELECT * FROM %1 WHERE userId=? ORDER BY createdAt DESC").arg(m_tableName));
        query.addBindValue(currentUserId);
        if(!query.exec()) {
            qDebug() << "❌ getDocuments failed:" << query.lastError().text();
            return result;
        }
    }

    int fetched = 0;
    while(query.next()) {
        if (fetched >= maxDocuments) {
            if (truncated)
                *truncated = true;
            qWarning() << "⚠️ getDocuments: truncated result set at" << maxDocuments
                       << "documents for user" << currentUserId;
            break;
        }
        result.append(documentFromQuery(query));
        ++fetched;
    }

    return result;
}

QString DocumentStorageDB::getClientNameById(int clientId) {
    QSqlQuery query(*db);
    query.prepare("SELECT firstName, middleName, lastName FROM clients WHERE id=?");
    query.addBindValue(clientId);
    if (!query.exec()) {
        qDebug() << "❌ getClientNameById query failed for id" << clientId << ":" << query.lastError().text();
        return QString();
    }

    if (query.next())
        return joinClientName(query.value(0).toString(), query.value(1).toString(), query.value(2).toString());

    qDebug() << "ℹ️ getClientNameById: no client found for id" << clientId;
    return QString();
}

int DocumentStorageDB::getClientIdByName(const QString &name) {
    QSqlQuery query(*db);
    if (!query.exec("SELECT id, firstName, middleName, lastName FROM clients")) {
        qDebug() << "❌ getClientIdByName query failed:" << query.lastError().text();
        return 0;
    }

    const QString target = normalizeName(name);
    while (query.next()) {
        const QString candidate = joinClientName(query.value(1).toString(), query.value(2).toString(), query.value(3).toString());
        if (!candidate.isEmpty() && normalizeName(candidate) == target)
            return query.value(0).toInt();
    }

    qDebug() << "ℹ️ getClientIdByName: no client found matching name" << name;
    return 0;
}

QString DocumentStorageDB::getUsernameById(int userId) {
    QSqlQuery query(*db);
    query.prepare("SELECT username FROM users WHERE id=?");
    query.addBindValue(userId);
    if(query.exec() && query.next()) return query.value(0).toString();
    qDebug() << "❌ getUsernameById failed or not found for id" << userId << ":" << query.lastError().text();
    return QString("unknown");
}

QVector<Document> DocumentStorageDB::getAllDocuments() {
    QVector<Document> result;
    QSqlQuery query(*db);
    if (!query.exec(QString("SELECT * FROM %1 ORDER BY createdAt DESC").arg(m_tableName))) {
        qDebug() << "❌ getAllDocuments failed:" << query.lastError().text();
        return result;
    }

    while (query.next())
        result.append(documentFromQuery(query));

    return result;
}

std::optional<Document> DocumentStorageDB::getDocumentById(int id) {
    QSqlQuery query(*db);
    query.prepare(QString("SELECT * FROM %1 WHERE id=?").arg(m_tableName));
    query.addBindValue(id);
    if (!query.exec()) {
        qDebug() << "❌ getDocumentById failed:" << query.lastError().text();
        return std::nullopt;
    }

    if (query.next())
        return documentFromQuery(query);
    return std::nullopt;
}
