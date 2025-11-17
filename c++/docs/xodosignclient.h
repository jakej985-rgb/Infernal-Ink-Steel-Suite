#pragma once

#include <QObject>
#include <QString>
#include <QJsonObject>
#include "domain/document.h"

class XodoSignClient : public QObject {
    Q_OBJECT

public:
    explicit XodoSignClient(const QString& apiKey, QObject *parent = nullptr);

    QJsonObject uploadDocument(const Document& document);
    QJsonObject createSignatureRequest(const QJsonObject& document, const QString& signerEmail);

private:
    QString m_apiKey;
};
