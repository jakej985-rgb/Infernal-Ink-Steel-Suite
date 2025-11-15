#include "inboxtab.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QSplitter>

InboxTab::InboxTab(FacebookClient* client, QWidget *parent)
    : ThemeableWidget(parent), m_facebookClient(client)
{
    m_conversationsList = new QListWidget(this);
    m_messagesList = new QListWidget(this);
    m_replyTextEdit = new QTextEdit(this);
    m_sendButton = new QPushButton("Send", this);

    m_replyTextEdit->setFixedHeight(100);

    QVBoxLayout* rightLayout = new QVBoxLayout();
    rightLayout->addWidget(m_messagesList);
    rightLayout->addWidget(m_replyTextEdit);
    rightLayout->addWidget(m_sendButton);

    QWidget* rightWidget = new QWidget(this);
    rightWidget->setLayout(rightLayout);

    QSplitter* splitter = new QSplitter(Qt::Horizontal, this);
    splitter->addWidget(m_conversationsList);
    splitter->addWidget(rightWidget);
    splitter->setStretchFactor(1, 2);

    QVBoxLayout* mainLayout = new QVBoxLayout(this);
    mainLayout->addWidget(splitter);
    setLayout(mainLayout);

    connect(m_facebookClient, &FacebookClient::conversationsFetched, this, &InboxTab::onConversationsFetched);
    connect(m_facebookClient, &FacebookClient::messagesFetched, this, &InboxTab::onMessagesFetched);
    connect(m_facebookClient, &FacebookClient::messageSent, this, &InboxTab::onMessageSent);
    connect(m_conversationsList, &QListWidget::itemClicked, this, &InboxTab::onConversationSelected);
    connect(m_sendButton, &QPushButton::clicked, this, &InboxTab::onSendClicked);
}

void InboxTab::setCurrentPage(const FacebookPageData& page) {
    m_currentPage = page;
    m_conversationsList->clear();
    m_messagesList->clear();
    m_facebookClient->fetchConversations(m_currentPage.id, m_currentPage.accessToken);
}

void InboxTab::onConversationsFetched(const QList<FacebookConversation>& conversations) {
    for (const auto& conv : conversations) {
        QListWidgetItem* item = new QListWidgetItem(conv.snippet, m_conversationsList);
        item->setData(Qt::UserRole, conv.id);
    }
}

void InboxTab::onMessagesFetched(const QList<FacebookMessage>& messages) {
    m_messagesList->clear();
    for (const auto& msg : messages) {
        m_messagesList->addItem(QString("%1: %2").arg(msg.from, msg.message));
    }
}

void InboxTab::onMessageSent(bool success) {
    if (success) {
        m_replyTextEdit->clear();
        onConversationSelected(m_conversationsList->currentItem()); // Refresh messages
    }
}

void InboxTab::onConversationSelected(QListWidgetItem* item) {
    if (item) {
        QString conversationId = item->data(Qt::UserRole).toString();
        m_facebookClient->fetchMessages(conversationId, m_currentPage.accessToken);
    }
}

void InboxTab::onSendClicked() {
    QString message = m_replyTextEdit->toPlainText();
    if (!message.isEmpty() && m_conversationsList->currentItem()) {
        QString conversationId = m_conversationsList->currentItem()->data(Qt::UserRole).toString();
        m_facebookClient->sendMessage(conversationId, message, m_currentPage.accessToken);
    }
}
