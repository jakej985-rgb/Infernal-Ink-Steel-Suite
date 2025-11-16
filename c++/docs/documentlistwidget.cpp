#include "documentlistwidget.h"
#include "helper/btnfader.h"
#include "db/databasemanager.h"
#include "db/documentstoragedb.h"
#include "domain/document.h"
#include "themes/theme.h"
#include "helper/thememanager.h"

#include <QFileInfo>
#include <QDebug>

DocumentListWidget::DocumentListWidget(QWidget* parent)
    : QListWidget(parent)
{
    setSelectionMode(QAbstractItemView::SingleSelection);
    setMouseTracking(true);

    connect(this, &QListWidget::itemClicked, this, &DocumentListWidget::onItemClicked);

    // Default hover & selection colors
    setStyleSheet(R"(
        QListWidget::item:hover { background-color: rgba(255, 215, 0, 0.2); }
        QListWidget::item:selected { background-color: rgba(30, 144, 255, 0.3); }
    )");
}

// -----------------------------------------------------------------------------
//  Load and filter documents
// -----------------------------------------------------------------------------
void DocumentListWidget::loadDocuments(int clientId, int userId)
{
    auto docs = DatabaseManager::instance()->documents().getAllDocuments();
    QVector<Document> filtered;

    for (const auto& d : docs) {
        if ((clientId == -1 || d.clientId == clientId) &&
            (userId == -1 || d.userId == userId))
        {
            filtered.push_back(d);
        }
    }

    refreshList(filtered);
}

// -----------------------------------------------------------------------------
//  Refresh visible list
// -----------------------------------------------------------------------------
void DocumentListWidget::refreshList(const QVector<Document>& docs)
{
    clear();

    for (const auto& d : docs) {
        auto* item = new QListWidgetItem(d.title);

        if (!d.exists())
            item->setForeground(Qt::red);

        item->setData(Qt::UserRole, d.id);
        addItem(item);

        animateItem(item);
    }
}

// -----------------------------------------------------------------------------
//  Animate item (for custom widgets only)
// -----------------------------------------------------------------------------
void DocumentListWidget::animateItem(QListWidgetItem* item)
{
    // ✅ Only works if you use custom item widgets
    QWidget* widget = itemWidget(item);
    if (widget)
        BtnFader::fadeIn(widget, 250);
    // Otherwise skip (plain QListWidgetItem has no QWidget)
}

// -----------------------------------------------------------------------------
//  CRUD operations
// -----------------------------------------------------------------------------
void DocumentListWidget::addDocument(const Document& doc)
{
    DatabaseManager::instance()->documents().addDocument(doc);
    loadDocuments();
}

void DocumentListWidget::removeDocument(int docId)
{
    DatabaseManager::instance()->documents().deleteDocument(docId);
    loadDocuments();
}

// -----------------------------------------------------------------------------
//  Handle click
// -----------------------------------------------------------------------------
void DocumentListWidget::onItemClicked(QListWidgetItem* item)
{
    int id = item->data(Qt::UserRole).toInt();

    // ✅ Use proper accessor for QSqlDatabase
    auto& db = DatabaseManager::instance()->getDatabase();
    auto opt = Document::getById(db, id);

    if (opt.has_value())
        emit documentSelected(opt.value());
}

// -----------------------------------------------------------------------------
//  Apply theme colors
// -----------------------------------------------------------------------------
void DocumentListWidget::applyTheme()
{
    Theme theme = ThemeManager::instance()->currentTheme();

    setStyleSheet(QString(
                      "QListWidget { background-color: %1; color: %2; font: %3pt \"%4\"; }"
                      "QListWidget::item:hover { background-color: rgba(255, 215, 0, 0.2); }"
                      "QListWidget::item:selected { background-color: rgba(30, 144, 255, 0.3); }"
                      ).arg(theme.background.name())
                      .arg(theme.text.name())
                      .arg(theme.font.pointSize())
                      .arg(theme.font.family()));
}
