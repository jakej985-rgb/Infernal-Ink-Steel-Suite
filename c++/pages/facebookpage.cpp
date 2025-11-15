#include "facebookpage.h"
#include "facebook/inboxtab.h"
#include "facebook/createposttab.h"
#include <QVBoxLayout>

FacebookPage::FacebookPage(FacebookClient* client, QWidget *parent)
    : ThemeableWidget(parent), m_facebookClient(client)
{
    m_tabWidget = new QTabWidget(this);

    InboxTab* inboxTab = new InboxTab(m_facebookClient, this);
    CreatePostTab* createPostTab = new CreatePostTab(m_facebookClient, this);

    m_tabWidget->addTab(inboxTab, "Inbox");
    m_tabWidget->addTab(createPostTab, "Create Post");

    QVBoxLayout* layout = new QVBoxLayout(this);
    layout->addWidget(m_tabWidget);
    setLayout(layout);
}

void FacebookPage::onPageSelected(const FacebookPageData& page) {
    InboxTab* inboxTab = static_cast<InboxTab*>(m_tabWidget->widget(0));
    inboxTab->setCurrentPage(page);

    CreatePostTab* createPostTab = static_cast<CreatePostTab*>(m_tabWidget->widget(1));
    createPostTab->setCurrentPage(page);
}
