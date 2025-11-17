// DocumentListWidget.h
#pragma once

#include <QListWidget>
#include "domain/document.h"
#include "db/databasemanager.h"
#include "helper/btnfader.h"
#include "themes/theme.h"

class DocumentListWidget : public QListWidget {
    Q_OBJECT
public:
    explicit DocumentListWidget(QWidget* parent = nullptr);
    void loadDocuments(int clientId = -1, int userId = -1);
    void addDocument(const Document& doc);
    void removeDocument(int docId);
    void applyTheme();

signals:
    void documentSelected(const Document& doc);

private slots:
    void onItemClicked(QListWidgetItem* item);

private:
    void refreshList(const QVector<Document>& docs);
    void animateItem(QListWidgetItem* item);
};
