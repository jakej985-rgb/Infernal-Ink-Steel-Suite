#include "documentspage.h"
#include <QStandardItem>
#include <QFileDialog>
#include <QFileInfo>
#include <QDir>
#include <QHBoxLayout>
#include <QVBoxLayout>
#include <QHeaderView>
#include <QInputDialog>
#include <QDesktopServices>
#include <QUrl>
#include <QMessageBox>
#include <QPdfDocument>
#include <QImage>
#include <QFile>
#include <QDateTime>
#include "helper/thememanager.h"

DocumentsPage::DocumentsPage(DocumentStorageDB *storage, int currentUserId, const QString &role, QWidget *parent)
    : ThemeableWidget(parent), storage(storage), currentUserId(currentUserId), currentUserRole(role)
{
    setupUI();
    loadDocuments();
}

void DocumentsPage::updateTheme()
{
    // The base class handles theme propagation.
    // The global stylesheet in styles.qss.template
    // is used to style QTableView.
}

void DocumentsPage::setupUI()
{
    tableView = new QTableView(this);
    tableModel = new QStandardItemModel(this);

    tableModel->setColumnCount(6);
    tableModel->setHeaderData(0, Qt::Horizontal, "Preview");
    tableModel->setHeaderData(1, Qt::Horizontal, "ID");
    tableModel->setHeaderData(2, Qt::Horizontal, "File Name");
    tableModel->setHeaderData(3, Qt::Horizontal, "Client ID");
    tableModel->setHeaderData(4, Qt::Horizontal, "Client Name");
    tableModel->setHeaderData(5, Qt::Horizontal, "Date Added");

    tableView->setModel(tableModel);
    tableView->setSelectionBehavior(QAbstractItemView::SelectRows);
    tableView->setEditTriggers(QAbstractItemView::NoEditTriggers);
    tableView->setSortingEnabled(true);
    tableView->setSelectionMode(QAbstractItemView::SingleSelection);
    tableView->setIconSize(QSize(64, 64));

    connect(tableView, &QTableView::doubleClicked, this, &DocumentsPage::documentAction);

    addButton = new QPushButton("Add Document");
    refreshButton = new QPushButton("Refresh");
    filterInput = new QLineEdit;
    filterInput->setPlaceholderText("Filter by Client ID or Name");
    filterButton = new QPushButton("Filter");

    QHBoxLayout *buttonLayout = new QHBoxLayout;
    buttonLayout->addWidget(addButton);
    buttonLayout->addWidget(refreshButton);
    buttonLayout->addWidget(filterInput);
    buttonLayout->addWidget(filterButton);

    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(tableView);
    mainLayout->addLayout(buttonLayout);
    setLayout(mainLayout);

    connect(addButton, &QPushButton::clicked, this, &DocumentsPage::addDocument);
    connect(refreshButton, &QPushButton::clicked, this, &DocumentsPage::refreshDocuments);
    connect(filterButton, &QPushButton::clicked, this, &DocumentsPage::filterDocuments);

    // Xodo Sign UI
    xodoSignClient = new XodoSignClient("YOUR_XODO_SIGN_API_KEY", this);
    signerEmailInput = new QLineEdit;
    signerEmailInput->setPlaceholderText("Signer's Email");
    sendSignatureRequestButton = new QPushButton("Send Signature Request");
    buttonLayout->addWidget(signerEmailInput);
    buttonLayout->addWidget(sendSignatureRequestButton);
    connect(sendSignatureRequestButton, &QPushButton::clicked, this, &DocumentsPage::sendSignatureRequest);
}

QIcon DocumentsPage::generateThumbnail(const QString &filePath)
{
    QPdfDocument pdf;
    if(pdf.load(filePath) == QPdfDocument::Error::None && pdf.pageCount() > 0)
    {
        QImage img = pdf.render(0, QSize(128,128));
        if(!img.isNull()) return QIcon(QPixmap::fromImage(img));
    }
    return QIcon(":/icons/pdf.png"); // fallback
}

void DocumentsPage::loadDocuments(const QString &filter)
{
    tableModel->removeRows(0, tableModel->rowCount());
    bool truncated = false;
    const auto limit = DocumentStorageDB::kDefaultFetchLimit;
    auto docs = storage->getDocuments(currentUserId, currentUserRole, limit, &truncated);

    if (truncated) {
        if (!truncatedWarningShown) {
            QMessageBox::warning(this,
                                 tr("Document list truncated"),
                                 tr("Only the first %1 documents are shown to prevent exhausting system memory. "
                                    "Please refine the filter to narrow the results.")
                                     .arg(limit));
            truncatedWarningShown = true;
        }
    } else {
        truncatedWarningShown = false;
    }
    int row = 0;

    for(const auto &doc : docs)
    {
        QString clientId = QString::number(doc.clientId);
        QString clientName = storage->getClientNameById(doc.clientId);

        if(!filter.isEmpty() &&
           !clientId.contains(filter, Qt::CaseInsensitive) &&
           !clientName.contains(filter, Qt::CaseInsensitive))
            continue;

        QIcon icon = generateThumbnail(doc.filePath);

        tableModel->setItem(row, 0, new QStandardItem(icon, ""));
        tableModel->setItem(row, 1, new QStandardItem(QString::number(doc.id)));
        tableModel->setItem(row, 2, new QStandardItem(doc.fileName()));
        tableModel->setItem(row, 3, new QStandardItem(clientId));
        tableModel->setItem(row, 4, new QStandardItem(clientName));
        tableModel->setItem(row, 5, new QStandardItem(doc.createdAt.toString(Qt::ISODate)));
        ++row;
    }

    tableView->setColumnWidth(0, 70);
    tableView->horizontalHeader()->setSectionResizeMode(2, QHeaderView::Stretch);
}

// --- Add Document ---
void DocumentsPage::addDocument()
{
    QString filePath = QFileDialog::getOpenFileName(this, "Select Document");
    if(filePath.isEmpty()) return;

    QString clientIdStr = QInputDialog::getText(this, "Client ID", "Enter Client ID:");
    if(clientIdStr.isEmpty()) return;

    int clientId = clientIdStr.toInt();
    QString clientName = storage->getClientNameById(clientId);
    if(clientName.isEmpty()) { QMessageBox::warning(this, "Error", "Client not found!"); return; }

    QString docType = QInputDialog::getText(this, "Document Type", "Enter type of document:");
    if(docType.isEmpty()) return;

    QString username = storage->getUsernameById(currentUserId);
    if(username.isEmpty()) username = "unknown";

    QString folderPath = QDir("Docs").filePath(username);
    folderPath = QDir(folderPath).filePath(clientName);

    QDir dir;
    if(!dir.exists(folderPath)) dir.mkpath(folderPath);

    QFileInfo fi(filePath);
    QString ext = fi.suffix();
    QString dateStr = QDateTime::currentDateTime().toString("yyyyMMdd");
    QString newFileName = QString("%1_%2_%3.%4").arg(clientName, docType, dateStr, ext);
    QString destPath = QDir(folderPath).filePath(newFileName);

    if(!QFile::rename(filePath, destPath)) { QMessageBox::warning(this, "Error", "Failed to move file!"); return; }

    Document doc;
    doc.userId = currentUserId;
    doc.clientId = clientId;
    doc.title = newFileName;
    doc.filePath = destPath;
    doc.createdAt = QDateTime::currentDateTime();

    if(storage->addDocument(doc)) loadDocuments();
    else QMessageBox::warning(this, "Error", "Failed to save document to database.");
}

// --- Document Actions (Delete, Rename, Open) ---
void DocumentsPage::documentAction(const QModelIndex &index)
{
    if(!index.isValid()) return;

    int row = index.row();
    int docId = tableModel->item(row, 1)->text().toInt();

    QVector<Document> docs = storage->getDocuments(currentUserId, currentUserRole);
    Document doc;
    bool found = false;
    for(const auto &d : docs) if(d.id == docId) { doc=d; found=true; break; }
    if(!found) { QMessageBox::warning(this, "Error", "Document not found!"); return; }

    DocumentActionDialog dlg(doc, this);
    if(dlg.exec() != QDialog::Accepted) return;

    switch(dlg.action())
    {
        case DocumentActionDialog::Delete:
            if(storage->deleteDocument(doc.id))
                loadDocuments();
            else
                QMessageBox::warning(this, "Error", "Failed to delete document.");
            break;

        case DocumentActionDialog::Rename:
        {
            QFileInfo fi(doc.filePath);
            QString newPath = fi.absolutePath() + "/" + dlg.newName();
            if(QFile::rename(doc.filePath, newPath))
            {
                doc.title = dlg.newName();
                doc.filePath = newPath;
                if(storage->updateDocument(doc))
                    loadDocuments();
                else
                    QMessageBox::warning(this, "Error", "Failed to update database record.");
            }
            else QMessageBox::warning(this, "Error", "Failed to rename file!");
            break;
        }

        case DocumentActionDialog::Open:
            if(QFile::exists(doc.filePath))
                QDesktopServices::openUrl(QUrl::fromLocalFile(doc.filePath));
            else
                QMessageBox::warning(this, "Error", "File not found!");
            break;

        default: break;
    }
}

void DocumentsPage::sendSignatureRequest()
{
    QModelIndexList selectedIndexes = tableView->selectionModel()->selectedIndexes();
    if(selectedIndexes.isEmpty())
    {
        QMessageBox::warning(this, "No Document Selected", "Please select a document to send for signature.");
        return;
    }

    int docId = tableModel->item(selectedIndexes.first().row(), 1)->text().toInt();
    std::optional<Document> docOpt = storage->getDocumentById(docId);

    if(!docOpt)
    {
        QMessageBox::critical(this, "Error", "Could not retrieve the document from the database.");
        return;
    }

    QString signerEmail = signerEmailInput->text();
    if(signerEmail.isEmpty())
    {
        QMessageBox::warning(this, "No Email", "Please enter the signer's email address.");
        return;
    }

    QJsonObject uploadResponse = xodoSignClient->uploadDocument(*docOpt);
    if(uploadResponse.isEmpty() || !uploadResponse.contains("id"))
    {
        QMessageBox::critical(this, "Upload Failed", "Failed to upload the document to XodoSign.");
        return;
    }

    QJsonObject signatureResponse = xodoSignClient->createSignatureRequest(uploadResponse, signerEmail);
    if(signatureResponse.isEmpty() || !signatureResponse.contains("id"))
    {
        QMessageBox::critical(this, "Signature Request Failed", "Failed to create the signature request.");
        return;
    }

    QMessageBox::information(this, "Success", "Signature request sent successfully!");
}

void DocumentsPage::refreshDocuments() { loadDocuments(); }
void DocumentsPage::filterDocuments() { loadDocuments(filterInput->text()); }