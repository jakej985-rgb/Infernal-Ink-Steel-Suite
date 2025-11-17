#pragma once
#include <QDialog>
#include <QLabel>
#include <QPushButton>
#include <QLineEdit>
#include <QVBoxLayout>
#include <QHBoxLayout>
#include "domain/document.h"

class DocumentActionDialog : public QDialog
{
    Q_OBJECT
public:
    enum Action { None, Delete, Rename, Open };

    explicit DocumentActionDialog(const Document &doc, QWidget *parent = nullptr);

    Action action() const { return m_action; }
    QString newName() const { return m_newName; }

private slots:
    void onDeleteClicked();
    void onRenameClicked();
    void onOpenClicked();

private:
    Document m_doc;
    Action m_action = None;
    QString m_newName;

    QLabel *infoLabel;
    QLineEdit *renameEdit;
    QPushButton *deleteBtn;
    QPushButton *renameBtn;
    QPushButton *openBtn;
    QPushButton *cancelBtn;

    void setupUI();
};