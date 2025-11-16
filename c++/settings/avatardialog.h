#pragma once
#include <QDialog>
#include <QString>

class AvatarDialog : public QDialog {
    Q_OBJECT
public:
    explicit AvatarDialog(QWidget* parent=nullptr);
    QString selectedPath() const { return m_path; }
private:
    QString m_path;
};