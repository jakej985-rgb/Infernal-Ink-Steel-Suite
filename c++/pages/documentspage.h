#ifndef DOCUMENTSPAGE_H
#define DOCUMENTSPAGE_H

#include <QWidget>
#include <QTableView>
#include <QStandardItemModel>
#include <QPushButton>
#include <QLineEdit>
#include <QModelIndex>
#include "db/documentstoragedb.h"
#include "dialog/documentactiondialog.h"
#include "themes/themeablewidget.h"
#include "helper/xodosignclient.h"

class DocumentsPage : public ThemeableWidget
{
    Q_OBJECT

public:
    explicit DocumentsPage(DocumentStorageDB *storage, int currentUserId, const QString &currentUserRole, QWidget *parent = nullptr);

private:
    void updateTheme() override;
    void setupUI();
    void loadDocuments(const QString &filter = "");
    QIcon generateThumbnail(const QString &filePath);

private slots:
    void addDocument();
    void documentAction(const QModelIndex &index);
    void refreshDocuments();
    void filterDocuments();
    void sendSignatureRequest();

private:
    DocumentStorageDB *storage;
    int currentUserId;
    QString currentUserRole;

    QTableView *tableView;
    QStandardItemModel *tableModel;
    QPushButton *addButton;
    QPushButton *refreshButton;
    QLineEdit *filterInput;
    QPushButton *filterButton;
    bool truncatedWarningShown = false;

    XodoSignClient *xodoSignClient;
    QLineEdit *signerEmailInput;
    QPushButton *sendSignatureRequestButton;
};

#endif // DOCUMENTSPAGE_H