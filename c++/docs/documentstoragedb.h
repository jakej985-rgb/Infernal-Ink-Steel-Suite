#ifndef DOCUMENTSTORAGEDB_H
#define DOCUMENTSTORAGEDB_H

#include <QSqlDatabase>
#include <QVector>
#include <QString>
#include <optional>
#include "domain/document.h"

class DocumentStorageDB {
public:
    static constexpr int kDefaultFetchLimit = 5000;

    explicit DocumentStorageDB(QSqlDatabase* database, const QString &tableName = "documents");

    bool createTable();
    bool addDocument(const Document &doc);
    bool updateDocument(const Document &doc);
    bool deleteDocument(int documentId);

    QVector<Document> getDocuments(int currentUserId, const QString &role,
                                   int maxDocuments = kDefaultFetchLimit,
                                   bool *truncated = nullptr);
    QVector<Document> getAllDocuments();
    std::optional<Document> getDocumentById(int id);


    QString getClientNameById(int clientId);
    int getClientIdByName(const QString &name);
    QString getUsernameById(int userId);

private:
    QSqlDatabase* db = nullptr;
    QString m_tableName;
};

#endif // DOCUMENTSTORAGEDB_H
