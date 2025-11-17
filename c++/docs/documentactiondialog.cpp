#include "documentactiondialog.h"
#include <QDesktopServices>
#include <QUrl>
#include <QMessageBox>
#include <QFileInfo>

DocumentActionDialog::DocumentActionDialog(const Document &doc, QWidget *parent)
    : QDialog(parent), m_doc(doc)
{
    setupUI();
}

void DocumentActionDialog::setupUI()
{
    setWindowTitle("Document Actions");
    setMinimumWidth(350);

    infoLabel = new QLabel(
        QString("Document: %1\nClient ID: %2\nAdded: %3")
            .arg(m_doc.title)
            .arg(m_doc.clientId)
            .arg(m_doc.createdAt.toString(Qt::ISODate))
    );

    renameEdit = new QLineEdit(this);
    renameEdit->setPlaceholderText("New file name (optional)");

    deleteBtn = new QPushButton("Delete", this);
    renameBtn = new QPushButton("Rename", this);
    openBtn   = new QPushButton("Open", this);
    cancelBtn = new QPushButton("Cancel", this);

    QHBoxLayout *btnLayout = new QHBoxLayout;
    btnLayout->addWidget(deleteBtn);
    btnLayout->addWidget(renameBtn);
    btnLayout->addWidget(openBtn);
    btnLayout->addWidget(cancelBtn);

    QVBoxLayout *mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(infoLabel);
    mainLayout->addWidget(renameEdit);
    mainLayout->addLayout(btnLayout);
    setLayout(mainLayout);

    connect(deleteBtn, &QPushButton::clicked, this, &DocumentActionDialog::onDeleteClicked);
    connect(renameBtn, &QPushButton::clicked, this, &DocumentActionDialog::onRenameClicked);
    connect(openBtn, &QPushButton::clicked, this, &DocumentActionDialog::onOpenClicked);
    connect(cancelBtn, &QPushButton::clicked, this, &QDialog::reject);
}

void DocumentActionDialog::onDeleteClicked()
{
    auto res = QMessageBox::question(this, "Confirm Delete",
                                     QString("Delete document '%1'?").arg(m_doc.title));
    if(res == QMessageBox::Yes) {
        m_action = Delete;
        accept();
    }
}

void DocumentActionDialog::onRenameClicked()
{
    QString newNameText = renameEdit->text().trimmed();
    if(newNameText.isEmpty()) {
        QMessageBox::warning(this, "Invalid Name", "Please enter a new file name.");
        return;
    }

    QFileInfo fi(m_doc.filePath);
    m_newName = QString("%1.%2").arg(newNameText, fi.suffix());
    m_action = Rename;
    accept();
}

void DocumentActionDialog::onOpenClicked()
{
    if(QFile::exists(m_doc.filePath)) {
        QDesktopServices::openUrl(QUrl::fromLocalFile(m_doc.filePath));
        m_action = Open;
        accept();
    } else {
        QMessageBox::warning(this, "File Missing", "File not found:\n" + m_doc.filePath);
    }
}