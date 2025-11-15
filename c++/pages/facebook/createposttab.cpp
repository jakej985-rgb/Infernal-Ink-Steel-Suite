#include "createposttab.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QFileDialog>
#include <QMessageBox>

CreatePostTab::CreatePostTab(FacebookClient* client, QWidget *parent)
    : ThemeableWidget(parent), m_facebookClient(client)
{
    m_postTextEdit = new QTextEdit(this);
    m_imagePathLineEdit = new QLineEdit(this);
    m_attachImageButton = new QPushButton("Attach Image", this);
    m_postButton = new QPushButton("Post", this);

    m_imagePathLineEdit->setReadOnly(true);

    QHBoxLayout* imageLayout = new QHBoxLayout();
    imageLayout->addWidget(m_imagePathLineEdit);
    imageLayout->addWidget(m_attachImageButton);

    QVBoxLayout* mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(m_postTextEdit);
    mainLayout->addLayout(imageLayout);
    mainLayout->addWidget(m_postButton);
    mainLayout->addStretch();

    setLayout(mainLayout);

    connect(m_attachImageButton, &QPushButton::clicked, this, [=](){
        QString imagePath = QFileDialog::getOpenFileName(this, "Select Image", "", "Images (*.png *.jpg *.jpeg)");
        if (!imagePath.isEmpty()) {
            m_imagePathLineEdit->setText(imagePath);
        }
    });

    connect(m_postButton, &QPushButton::clicked, this, &CreatePostTab::onPostButtonClicked);
    connect(m_facebookClient, &FacebookClient::postCreated, this, &CreatePostTab::onPostCreated);
}

void CreatePostTab::setCurrentPage(const FacebookPageData& page) {
    m_currentPage = page;
}

void CreatePostTab::onPostButtonClicked() {
    QString message = m_postTextEdit->toPlainText();
    QString imagePath = m_imagePathLineEdit->text();
    m_facebookClient->createPost(m_currentPage.id, message, imagePath, m_currentPage.accessToken);
}

void CreatePostTab::onPostCreated(bool success) {
    if (success) {
        m_postTextEdit->clear();
        m_imagePathLineEdit->clear();
        QMessageBox::information(this, "Success", "Post created successfully!");
    } else {
        QMessageBox::warning(this, "Error", "Failed to create post.");
    }
}
